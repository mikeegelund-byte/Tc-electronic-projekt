#!/usr/bin/env python3
import argparse
import json
import os
import sys

LARGE_OFFSET = 1 << 24


def encode_u7x4(value: int) -> list[int]:
    if value < 0 or value > (1 << 28) - 1:
        raise ValueError(f"value out of range for u7x4: {value}")
    return [
        value & 0x7F,
        (value >> 7) & 0x7F,
        (value >> 14) & 0x7F,
        (value >> 21) & 0x7F,
    ]


def encode_s7x4(value: int) -> list[int]:
    return encode_u7x4(value + LARGE_OFFSET)


def set_u7x4(buf: bytearray, offset: int, value: int) -> None:
    b = encode_u7x4(value)
    buf[offset:offset + 4] = bytes(b)


def set_s7x4(buf: bytearray, offset: int, value: int) -> None:
    b = encode_s7x4(value)
    buf[offset:offset + 4] = bytes(b)


def set_bool(buf: bytearray, offset: int, value: bool) -> None:
    set_u7x4(buf, offset, 1 if value else 0)


def load_map(path: str) -> dict:
    with open(path, "r", encoding="utf-8") as f:
        return json.load(f)


def parse_params_json(path: str | None) -> dict:
    if not path:
        return {}
    with open(path, "r", encoding="utf-8") as f:
        data = json.load(f)
    if not isinstance(data, dict):
        raise ValueError("params json must be an object")
    return data


def parse_set_list(pairs: list[str] | None) -> dict:
    params: dict = {}
    for item in pairs or []:
        if "=" not in item:
            raise ValueError(f"Invalid --set value: {item}")
        key, value = item.split("=", 1)
        value = value.strip()
        if value.lower() in ("true", "false"):
            parsed: object = value.lower() == "true"
        else:
            try:
                parsed = int(value)
            except ValueError as exc:
                raise ValueError(f"Non-integer value for {key}: {value}") from exc
        params[key.strip()] = parsed
    return params


def write_name(buf: bytearray, offset: int, length: int, name: str) -> None:
    safe = name.encode("ascii", errors="ignore")[:length]
    buf[offset:offset + length] = safe.ljust(length, b" ")


def create_blank_preset(device_id: int, preset_number: int, name: str) -> bytearray:
    buf = bytearray(521)
    buf[0] = 0xF0
    buf[1] = 0x00
    buf[2] = 0x20
    buf[3] = 0x1F
    buf[4] = device_id & 0x7F
    buf[5] = 0x63
    buf[6] = 0x20
    buf[7] = 0x01
    buf[8] = preset_number & 0x7F
    write_name(buf, 9, 24, name)
    buf[520] = 0xF7
    return buf


def apply_params(buf: bytearray, params: dict, mapping: dict) -> None:
    param_map = mapping.get("parameters", {})
    for key, value in params.items():
        if key not in param_map:
            raise KeyError(f"Unknown parameter: {key}")
        spec = param_map[key]
        ptype = spec.get("type")
        if ptype == "bool":
            set_bool(buf, spec["offset"], bool(value))
            continue
        if not isinstance(value, int):
            raise ValueError(f"Parameter {key} must be an integer")
        min_v = spec.get("min")
        max_v = spec.get("max")
        if min_v is not None and value < min_v:
            raise ValueError(f"{key} below min {min_v}: {value}")
        if max_v is not None and value > max_v:
            raise ValueError(f"{key} above max {max_v}: {value}")
        if ptype == "u7x4":
            set_u7x4(buf, spec["offset"], value)
        elif ptype == "s7x4":
            set_s7x4(buf, spec["offset"], value)
        else:
            raise ValueError(f"Unsupported parameter type for {key}: {ptype}")


def write_checksum(buf: bytearray, mapping: dict) -> None:
    start = mapping.get("checksum_start", 34)
    end = mapping.get("checksum_end", 517)
    checksum = sum(buf[start:end + 1]) & 0x7F
    buf[mapping.get("checksum_offset", 518)] = checksum


def main() -> int:
    parser = argparse.ArgumentParser(
        description="Build a Nova System preset .syx from a base preset and parameter changes."
    )
    parser.add_argument(
        "--map",
        default=os.path.join(os.path.dirname(__file__), "preset_map.json"),
        help="Path to preset_map.json",
    )
    parser.add_argument("--base", help="Base .syx preset to modify")
    parser.add_argument("--out", required=True, help="Output .syx path")
    parser.add_argument("--preset-number", type=int, help="Target preset number (31-90)")
    parser.add_argument("--name", help="Preset name (24 chars max)")
    parser.add_argument("--device-id", type=int, default=0, help="Device ID (0-127)")
    parser.add_argument("--params-json", help="JSON file with parameter overrides")
    parser.add_argument(
        "--set",
        action="append",
        help="Override parameter, e.g. DriveGain=35 (can be repeated)",
    )
    args = parser.parse_args()

    mapping = load_map(args.map)
    params = parse_params_json(args.params_json)
    params.update(parse_set_list(args.set))

    preset_number = args.preset_number
    if preset_number is None:
        preset_number = 31
    if preset_number < 31 or preset_number > 90:
        raise ValueError("preset-number must be between 31 and 90")

    if args.base:
        with open(args.base, "rb") as f:
            buf = bytearray(f.read())
        if len(buf) != 521:
            raise ValueError("Base .syx must be 521 bytes")
    else:
        base_name = args.name or "LLM Preset"
        buf = create_blank_preset(args.device_id, preset_number, base_name)

    if args.preset_number is not None:
        buf[mapping.get("preset_number_offset", 8)] = preset_number & 0x7F

    if args.name:
        write_name(
            buf,
            mapping.get("preset_name_offset", 9),
            mapping.get("preset_name_length", 24),
            args.name,
        )

    apply_params(buf, params, mapping)
    write_checksum(buf, mapping)
    buf[0] = 0xF0
    buf[520] = 0xF7

    out_dir = os.path.dirname(args.out)
    if out_dir and not os.path.isdir(out_dir):
        raise ValueError(f"Output directory does not exist: {out_dir}")

    with open(args.out, "wb") as f:
        f.write(buf)

    print(f"Wrote preset: {args.out}")
    return 0


if __name__ == "__main__":
    try:
        raise SystemExit(main())
    except Exception as exc:
        print(f"Error: {exc}", file=sys.stderr)
        raise SystemExit(1)

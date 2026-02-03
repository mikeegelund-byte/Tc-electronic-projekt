/*
 * Decompiled with CFR 0.152.
 */
package nova;

import java.awt.BorderLayout;
import java.util.Arrays;
import java.util.List;
import javax.swing.Box;
import javax.swing.JLabel;
import javax.swing.JPanel;
import nova.Constants;
import nova.NibbleException;
import nova.OutOfBoundsException;
import nova.Patch;
import nova.PrintablePanel;

/*
 * This class specifies class file version 49.0 but uses Java 6 signatures.  Assumed Java 6.
 */
public class Nibble
extends JPanel {
    private byte[] bytes;
    private int value;
    private String stringValue = "unknown";
    private int offset = 0;
    private boolean editable = true;
    private PrintablePanel parent = null;
    static final String[] test = new String[]{"Test", "Test"};
    private static final int MINVALUE = -16384;
    private static final int MAXVALUE = 16383;
    static final int TYPE_DECI = 1;
    static final int TYPE_ONOFF = 2;
    static final int TYPE_ROUTING = 3;
    static final int TYPE_MAPPARAM = 4;
    static final int TYPE_OFFON = 5;
    static final int TYPE_PRESET = 6;
    static final int TYPE_CPRESET = 7;
    static final int TYPE_ATTACK = 10;
    static final int TYPE_RATIO = 11;
    static final int TYPE_RELEASE = 12;
    static final int TYPE_EQFREQ = 13;
    static final int TYPE_EQWIDTH = 14;
    static final int TYPE_SPEED = 20;
    static final int TYPE_TEMPO = 21;
    static final int TYPE_HICUT = 22;
    static final int TYPE_LOCUT = 24;
    static final int TYPE_HILO = 25;
    static final int TYPE_HARDSOFT = 26;
    static final int TYPE_SOFTHARD = 27;
    static final int TYPE_LOHI = 28;
    static final int TYPE_HICOL = 30;
    static final int TYPE_LOCOL = 31;
    static final int TYPE_SHAPE = 32;
    static final int TYPE_SIZE = 33;
    static final int TYPE_KEY = 40;
    static final int TYPE_SCALE = 41;
    static final int TYPE_DEGREES = 42;
    static final int TYPE_UPDOWN = 43;
    static final int TYPE_DOWNUP = 44;
    static final int TYPE_COMP = 50;
    static final int TYPE_DRIVE = 51;
    static final int TYPE_DELAY = 52;
    static final int TYPE_REVERB = 53;
    static final int TYPE_MOD = 54;
    static final int TYPE_PITCH = 55;
    static final int TYPE_GATE = 56;
    static final int TYPE_PEDAL = 60;
    static final int TYPE_PEDALMASTER = 61;
    static final int TYPE_TAPMASTER = 62;
    static final int TYPE_FOOTSWITCH = 63;
    static final int TYPE_INPUTSRC = 64;
    static final int TYPE_DCLOCK = 65;
    static final int TYPE_DITHER = 66;
    static final int TYPE_OUTPUTRANGE = 67;
    static final int TYPE_VOLUMEPOS = 68;
    static final int TYPE_TUNEROUT = 69;
    static final int TYPE_MIDI_CC = 70;
    static final int TYPE_MIDI_CHANNEL = 71;
    static final int TYPE_MIDI_SYSEX = 72;
    static final int TYPE_MIDI_MAP = 73;
    static final int TYPE_TUNERMODE = 74;
    static final int TYPE_TUNERRANGE = 75;
    static final int TYPE_IMPEDANCE = 76;
    static final int TYPE_DECI_01_50 = 90;
    static final int TYPE_DECI_01_20 = 91;
    static final int TYPE_N100_OFF_0 = 92;
    static final int TYPE_INT = 100;
    static final int TYPE_N200_200 = 101;
    static final int TYPE_N100_0 = 102;
    static final int TYPE_N100_100 = 103;
    static final int TYPE_N99_15 = 104;
    static final int TYPE_N60_0 = 105;
    static final int TYPE_N50_0 = 106;
    static final int TYPE_N50_50 = 107;
    static final int TYPE_N30_0 = 108;
    static final int TYPE_N25_25 = 109;
    static final int TYPE_N12_12 = 110;
    static final int TYPE_0_10 = 111;
    static final int TYPE_0_24 = 112;
    static final int TYPE_0_30 = 113;
    static final int TYPE_N40_0 = 114;
    static final int TYPE_0_90 = 115;
    static final int TYPE_0_100 = 116;
    static final int TYPE_0_120 = 117;
    static final int TYPE_0_200 = 118;
    static final int TYPE_0_1800 = 119;
    static final int TYPE_1_2 = 120;
    static final int TYPE_1_10 = 121;
    static final int TYPE_1_20 = 122;
    static final int TYPE_100_3000 = 123;
    static final int TYPE_0_18 = 124;
    static final int TYPE_3_200 = 125;
    static final int TYPE_0_350 = 126;
    static final int TYPE_N2400_2400 = 127;
    static final int TYPE_0_50 = 128;
    static final int TYPE_N100_6 = 129;
    static final int TYPE_420_460 = 130;
    static final int TYPE_N6_18 = 131;
    static final int TYPE_N99_0 = 132;
    static final int TYPE_N99_12 = 133;
    private JLabel label = new JLabel();
    private String name = "";
    private int type = 100;
    private Box vbox = Box.createVerticalBox();
    private String[] typeList = Constants.ONOFF;
    private List<String> valueList;

    public Nibble(PrintablePanel printablePanel, byte[] byArray) throws NibbleException {
        this.parent = printablePanel;
        this.bytes = new byte[4];
        for (int i = 0; i < 4; ++i) {
            this.bytes[i] = byArray[i];
        }
        this.value = Nibble.toInteger(this.bytes);
        this.setLayout(new BorderLayout());
        this.setType(2);
    }

    public Nibble(PrintablePanel printablePanel, int n) throws NibbleException {
        this.parent = printablePanel;
        this.value = n;
        this.bytes = Nibble.toBytes(this.value);
        this.setLayout(new BorderLayout());
        this.setType(2);
    }

    public Nibble(PrintablePanel printablePanel, byte[] byArray, int n) throws NibbleException {
        this.parent = printablePanel;
        this.bytes = new byte[4];
        for (int i = 0; i < 4; ++i) {
            this.bytes[i] = byArray[i];
        }
        this.value = Nibble.toInteger(this.bytes);
        this.offset = n;
        this.setLayout(new BorderLayout());
        this.setType(100);
    }

    public static int toInteger(byte[] byArray) throws NibbleException {
        int n = 0;
        if (byArray[3] == 0) {
            n += byArray[0];
            n += byArray[1] * 128;
        } else {
            n += byArray[0] - 128;
            n += (byArray[1] - 127) * 128;
        }
        return n;
    }

    public static byte[] toBytes(int n) throws NibbleException {
        byte[] byArray = new byte[4];
        if (n >= 0) {
            byArray[0] = (byte)(n % 128);
            byArray[1] = (byte)(n / 128);
            byArray[2] = 0;
            byArray[3] = 0;
        } else {
            byArray[0] = (byte)(128 - -n % 128);
            byArray[1] = (byte)(n / 128 + 127);
            byArray[2] = 127;
            byArray[3] = 7;
        }
        return byArray;
    }

    public byte[] getBytes() {
        return this.bytes;
    }

    public int getValue() {
        return this.value;
    }

    public String getStringValue() {
        return this.stringValue;
    }

    public char getASCII() {
        return (char)(this.value % 128);
    }

    public void setValue(int n) throws NibbleException {
        this.value = n;
        this.bytes = Nibble.toBytes(this.value);
        if (this.type != 73) {
            this.decode();
        }
    }

    /*
     * WARNING - Removed try catching itself - possible behaviour change.
     */
    public void setValue(String string) {
        int n = 0;
        try {
            n = this.encode(string);
        }
        catch (OutOfBoundsException outOfBoundsException) {
            n = this.getDefaultValue();
            this.log(outOfBoundsException.getMessage());
        }
        finally {
            try {
                this.setValue(n);
            }
            catch (NibbleException nibbleException) {
                nibbleException.printStackTrace();
            }
        }
    }

    private int encode(String string) throws OutOfBoundsException {
        int n = this.valueList.indexOf(string);
        if (n >= 0) {
            switch (this.type) {
                case 4: {
                    List<String> list = Arrays.asList(this.typeList);
                    this.value = list.indexOf(string) - 1;
                    break;
                }
                case 7: {
                    this.value = n + 1;
                    break;
                }
                case 12: {
                    this.value = n + 3;
                    break;
                }
                case 13: {
                    this.value = n + 25;
                    break;
                }
                case 14: {
                    this.value = n + 3;
                    break;
                }
                case 42: {
                    this.value = n - 12;
                    break;
                }
                case 101: {
                    this.value = n - 200;
                    break;
                }
                case 102: {
                    this.value = n - 100;
                    break;
                }
                case 92: {
                    this.value = n - 100;
                    break;
                }
                case 103: {
                    this.value = n - 100;
                    break;
                }
                case 104: {
                    this.value = n - 99;
                    break;
                }
                case 132: {
                    this.value = n - 99;
                    break;
                }
                case 133: {
                    this.value = n - 99;
                    break;
                }
                case 105: {
                    this.value = n - 60;
                    break;
                }
                case 106: {
                    this.value = n - 50;
                    break;
                }
                case 107: {
                    this.value = n - 50;
                    break;
                }
                case 108: {
                    this.value = n - 30;
                    break;
                }
                case 131: {
                    this.value = n - 6;
                    break;
                }
                case 109: {
                    this.value = n - 25;
                    break;
                }
                case 110: {
                    this.value = n - 12;
                    break;
                }
                case 120: {
                    this.value = n + 1;
                    break;
                }
                case 121: {
                    this.value = n + 1;
                    break;
                }
                case 122: {
                    this.value = n + 1;
                    break;
                }
                case 123: {
                    this.value = n + 100;
                    break;
                }
                case 125: {
                    this.value = n + 3;
                    break;
                }
                case 127: {
                    this.value = n - 2400;
                    break;
                }
                case 129: {
                    this.value = n - 100;
                    break;
                }
                case 130: {
                    this.value = n + 420;
                    break;
                }
                case 91: {
                    this.value = n + 1;
                    break;
                }
                default: {
                    this.value = n;
                    break;
                }
            }
        } else {
            throw new OutOfBoundsException(string, this);
        }
        return this.value;
    }

    void setMidiValue(int[] nArray) {
        int n = nArray[2];
        int n2 = nArray[1] % 64 * 2;
        int n3 = nArray[0] % 32 * 4 + nArray[1] / 64;
        int n4 = nArray[0] / 32;
        this.bytes[0] = (byte)n;
        this.bytes[1] = (byte)n2;
        this.bytes[2] = (byte)n3;
        this.bytes[3] = (byte)n4;
    }

    public void setBytes(byte[] byArray) throws NibbleException {
        for (int i = 0; i < 4; ++i) {
            this.bytes[i] = byArray[i];
        }
        this.value = Nibble.toInteger(this.bytes);
        this.decode();
    }

    private static void check(byte[] byArray) throws NibbleException {
        boolean bl = false;
        if (byArray == null) {
            throw new NibbleException(0, byArray);
        }
        if (byArray.length != 4) {
            throw new NibbleException(1, byArray);
        }
        if (byArray[3] != 0 && byArray[3] != 7) {
            throw new NibbleException(2, byArray);
        }
        if (byArray[3] == 0 && byArray[2] != 0) {
            throw new NibbleException(3, byArray);
        }
        if (byArray[3] == 7 && byArray[2] != 127) {
            throw new NibbleException(3, byArray);
        }
        if ((byArray[0] & 0x80) == 128 || (byArray[1] & 0x80) == 128) {
            throw new NibbleException(4, byArray);
        }
    }

    private static void check(int n) throws NibbleException {
        if (n < -16384) {
            throw new NibbleException(5, n);
        }
        if (n > 16383) {
            throw new NibbleException(6, n);
        }
    }

    @Override
    public String toString() {
        String string = "" + this.value + " =";
        for (int i = 0; i < 4; ++i) {
            string = string + " " + Integer.toString((this.bytes[i] & 0xFF) + 256, 16).substring(1);
        }
        return string.toUpperCase();
    }

    public String toHexa() {
        String string = "";
        for (int i = 0; i < 4; ++i) {
            string = string + Integer.toString((this.bytes[i] & 0xFF) + 256, 16).substring(1);
        }
        return string.toUpperCase();
    }

    public String toDecimal() {
        String string = "" + this.value;
        return string;
    }

    public String toAscii() {
        String string = "" + (char)this.bytes[0];
        return string;
    }

    public String toXML() {
        String string = "";
        String string2 = this.toXMLOpen();
        string2 = string2 + this.stringValue;
        string2 = string2 + this.toXMLClose();
        return string2;
    }

    public String toXMLOpen() {
        String string = "<param ";
        string = string + "name=\"" + this.name + "\" ";
        string = string + "value=\"" + this.toHexa() + "\" ";
        string = string + "type=\"" + this.type + "\"";
        string = string + ">";
        return string;
    }

    public String toXMLClose() {
        String string = "</param>\n";
        return string;
    }

    public String toHTML() {
        String string = "";
        String string2 = "          <BR/>" + this.name + ": <I>" + this.stringValue + "</I>\n";
        return string2;
    }

    int getType() {
        return this.type;
    }

    String[] getTypeList() {
        return this.typeList;
    }

    void setTypeList(String[] stringArray) {
        this.typeList = stringArray;
        if (this.type != 73) {
            this.decode();
        }
    }

    public void save(byte[] byArray) {
        for (int i = 0; i < 4; ++i) {
            byArray[this.offset + i] = this.bytes[i];
        }
    }

    public void decode() {
        int n = this.value;
        String string = "";
        if (this.type == 100) {
            this.stringValue = string;
        } else if (this.type > 100) {
            this.stringValue = "" + this.value;
            if (this.valueList.indexOf(this.stringValue) < 0) {
                this.value = this.getDefaultValue();
                this.stringValue = this.typeList[0];
                OutOfBoundsException outOfBoundsException = new OutOfBoundsException(n, this);
                this.log(outOfBoundsException.getMessage());
            }
        } else {
            try {
                switch (this.type) {
                    case 90: {
                        string = "" + this.value / 10 + "." + this.value % 10;
                        break;
                    }
                    case 91: {
                        string = "" + this.value / 10 + "." + this.value % 10;
                        break;
                    }
                    case 92: {
                        string = this.typeList[this.value + 100];
                        break;
                    }
                    case 42: {
                        string = this.typeList[this.value + 12];
                        break;
                    }
                    case 12: {
                        string = this.typeList[this.value - 3];
                        break;
                    }
                    case 13: {
                        string = this.typeList[this.value - 25];
                        break;
                    }
                    case 14: {
                        string = this.typeList[this.value - 3];
                        break;
                    }
                    case 7: {
                        string = this.typeList[this.value - 1];
                        break;
                    }
                    case 4: {
                        string = this.typeList[this.value + 1];
                        break;
                    }
                    default: {
                        string = this.typeList[this.value];
                    }
                }
                this.stringValue = string;
            }
            catch (ArrayIndexOutOfBoundsException arrayIndexOutOfBoundsException) {
                this.value = this.getDefaultValue();
                this.stringValue = this.typeList[0];
                OutOfBoundsException outOfBoundsException = new OutOfBoundsException(n, this);
                this.log(outOfBoundsException.getMessage());
            }
        }
    }

    protected void setValueList(List<String> list) {
        this.valueList = list;
    }

    @Override
    public String getName() {
        return this.name;
    }

    int getMapMidiValue(int n) {
        int n2 = 0;
        switch (n) {
            case 0: {
                n2 = this.getMapMidiValue1();
                break;
            }
            case 1: {
                n2 = this.getMapMidiValue2();
                break;
            }
            case 2: {
                n2 = this.getMapMidiValue3();
                break;
            }
            default: {
                n2 = 0;
            }
        }
        return n2;
    }

    int getMapMidiValue1() {
        int n = this.bytes[2] & 0xFF;
        int n2 = this.bytes[3] & 0xFF;
        int n3 = n / 4 + 32 * (n2 % 8);
        return n3;
    }

    int getMapMidiValue2() {
        int n = this.bytes[1] & 0xFF;
        int n2 = this.bytes[2] & 0xFF;
        int n3 = n / 2 + 64 * (n2 % 4);
        return n3;
    }

    int getMapMidiValue3() {
        int n = this.bytes[0] & 0xFF;
        int n2 = this.bytes[1] & 0xFF;
        int n3 = n + 128 * (n2 % 2);
        return n3;
    }

    void updateNibble(String string, int n) {
        this.name = string;
        this.setType(n);
    }

    void setType(int n) {
        this.type = n;
        switch (this.type) {
            case 100: {
                this.typeList = null;
                break;
            }
            case 2: {
                this.typeList = Constants.ONOFF;
                break;
            }
            case 5: {
                this.typeList = Constants.OFFON;
                break;
            }
            case 3: {
                this.typeList = Constants.ROUTING;
                break;
            }
            case 4: {
                Patch patch = (Patch)this.parent;
                this.setValueList(patch.getExpressionPedalValueList());
                this.typeList = patch.exp_pedal_valueList;
                break;
            }
            case 10: {
                this.typeList = Constants.ATTACK;
                break;
            }
            case 11: {
                this.typeList = Constants.RATIO;
                break;
            }
            case 12: {
                this.typeList = Constants.RELEASE;
                break;
            }
            case 13: {
                this.typeList = Constants.EQFREQ;
                break;
            }
            case 14: {
                this.typeList = Constants.EQWIDTH;
                break;
            }
            case 20: {
                this.typeList = Constants.SPEED;
                break;
            }
            case 21: {
                this.typeList = Constants.TEMPO;
                break;
            }
            case 22: {
                this.typeList = Constants.HICUT;
                break;
            }
            case 24: {
                this.typeList = Constants.LOCUT;
                break;
            }
            case 25: {
                this.typeList = Constants.HILO;
                break;
            }
            case 28: {
                this.typeList = Constants.LOHI;
                break;
            }
            case 27: {
                this.typeList = Constants.SOFTHARD;
                break;
            }
            case 26: {
                this.typeList = Constants.HARDSOFT;
                break;
            }
            case 30: {
                this.typeList = Constants.HICOL;
                break;
            }
            case 31: {
                this.typeList = Constants.LOCOL;
                break;
            }
            case 32: {
                this.typeList = Constants.SHAPE;
                break;
            }
            case 33: {
                this.typeList = Constants.SIZE;
                break;
            }
            case 40: {
                this.typeList = Constants.KEY;
                break;
            }
            case 41: {
                this.typeList = Constants.SCALE;
                break;
            }
            case 42: {
                this.typeList = Constants.DEGREES;
                break;
            }
            case 43: {
                this.typeList = Constants.UPDOWN;
                break;
            }
            case 44: {
                this.typeList = Constants.DOWNUP;
                break;
            }
            case 50: {
                this.typeList = Constants.COMP;
                break;
            }
            case 51: {
                this.typeList = Constants.DRIVE;
                break;
            }
            case 52: {
                this.typeList = Constants.DELAY;
                break;
            }
            case 53: {
                this.typeList = Constants.REVERB;
                break;
            }
            case 54: {
                this.typeList = Constants.MOD;
                break;
            }
            case 55: {
                this.typeList = Constants.PITCH;
                break;
            }
            case 56: {
                this.typeList = Constants.SOFTHARD;
                break;
            }
            case 60: {
                this.typeList = Constants.PEDAL;
                break;
            }
            case 61: {
                this.typeList = Constants.PEDALMASTER;
                break;
            }
            case 62: {
                this.typeList = Constants.TAPMASTER;
                break;
            }
            case 63: {
                this.typeList = Constants.FOOTSWITCH;
                break;
            }
            case 64: {
                this.typeList = Constants.INPUTSRC;
                break;
            }
            case 65: {
                this.typeList = Constants.DCLOCK;
                break;
            }
            case 66: {
                this.typeList = Constants.DITHER;
                break;
            }
            case 67: {
                this.typeList = Constants.OUTPUTRANGE;
                break;
            }
            case 68: {
                this.typeList = Constants.VOLUMEPOS;
                break;
            }
            case 69: {
                this.typeList = Constants.TUNEROUT;
                break;
            }
            case 74: {
                this.typeList = Constants.TUNERMODE;
                break;
            }
            case 75: {
                this.typeList = Constants.TUNERRANGE;
                break;
            }
            case 76: {
                this.typeList = Constants.IMPEDANCE;
                break;
            }
            case 70: {
                this.typeList = Constants.MIDI_CC;
                break;
            }
            case 71: {
                this.typeList = Constants.MIDI_CHANNEL;
                break;
            }
            case 72: {
                this.typeList = Constants.MIDI_SYSEX;
                break;
            }
            case 6: {
                this.typeList = Constants.PRESET;
                break;
            }
            case 7: {
                this.typeList = Constants.CPRESET;
                break;
            }
            case 101: {
                this.typeList = Constants.TYPE_N200_200;
                break;
            }
            case 102: {
                this.typeList = Constants.TYPE_N100_0;
                break;
            }
            case 92: {
                this.typeList = Constants.TYPE_N100_OFF_0;
                break;
            }
            case 103: {
                this.typeList = Constants.TYPE_N100_100;
                break;
            }
            case 104: {
                this.typeList = Constants.TYPE_N99_15;
                break;
            }
            case 133: {
                this.typeList = Constants.TYPE_N99_12;
                break;
            }
            case 132: {
                this.typeList = Constants.TYPE_N99_0;
                break;
            }
            case 105: {
                this.typeList = Constants.TYPE_N60_0;
                break;
            }
            case 106: {
                this.typeList = Constants.TYPE_N50_0;
                break;
            }
            case 107: {
                this.typeList = Constants.TYPE_N50_50;
                break;
            }
            case 114: {
                this.typeList = Constants.TYPE_N40_0;
                break;
            }
            case 108: {
                this.typeList = Constants.TYPE_N30_0;
                break;
            }
            case 109: {
                this.typeList = Constants.TYPE_N25_25;
                break;
            }
            case 110: {
                this.typeList = Constants.TYPE_N12_12;
                break;
            }
            case 111: {
                this.typeList = Constants.TYPE_0_10;
                break;
            }
            case 112: {
                this.typeList = Constants.TYPE_0_24;
                break;
            }
            case 113: {
                this.typeList = Constants.TYPE_0_30;
                break;
            }
            case 90: {
                this.typeList = Constants.TYPE_DECI_01_50;
                break;
            }
            case 115: {
                this.typeList = Constants.TYPE_0_90;
                break;
            }
            case 116: {
                this.typeList = Constants.TYPE_0_100;
                break;
            }
            case 117: {
                this.typeList = Constants.TYPE_0_120;
                break;
            }
            case 118: {
                this.typeList = Constants.TYPE_0_200;
                break;
            }
            case 119: {
                this.typeList = Constants.TYPE_0_1800;
                break;
            }
            case 120: {
                this.typeList = Constants.TYPE_1_2;
                break;
            }
            case 121: {
                this.typeList = Constants.TYPE_1_10;
                break;
            }
            case 122: {
                this.typeList = Constants.TYPE_1_20;
                break;
            }
            case 123: {
                this.typeList = Constants.TYPE_100_3000;
                break;
            }
            case 91: {
                this.typeList = Constants.TYPE_DECI_01_20;
                break;
            }
            case 124: {
                this.typeList = Constants.TYPE_0_18;
                break;
            }
            case 125: {
                this.typeList = Constants.TYPE_3_200;
                break;
            }
            case 127: {
                this.typeList = Constants.TYPE_N2400_2400;
                break;
            }
            case 126: {
                this.typeList = Constants.TYPE_0_350;
                break;
            }
            case 128: {
                this.typeList = Constants.TYPE_0_50;
                break;
            }
            case 129: {
                this.typeList = Constants.TYPE_N100_6;
                break;
            }
            case 130: {
                this.typeList = Constants.TYPE_420_460;
                break;
            }
            case 131: {
                this.typeList = Constants.TYPE_N6_18;
                break;
            }
            default: {
                this.typeList = null;
            }
        }
        if (this.typeList != null && this.type != 4) {
            this.valueList = Arrays.asList(this.typeList);
        }
        if (this.type != 73) {
            this.decode();
        }
    }

    public void setEditable(boolean bl) {
        this.editable = bl;
    }

    public boolean isEditable() {
        return this.editable;
    }

    public static void main(String[] stringArray) {
        try {
            int n = Integer.parseInt(stringArray[0]);
            Nibble nibble = new Nibble(null, n);
            System.out.println(nibble.toString());
            byte[] byArray = nibble.getBytes();
            Nibble nibble2 = new Nibble(null, byArray);
            System.out.println(nibble2.toString());
            System.out.println(nibble2.toAscii());
            System.out.println(nibble2.toDecimal());
            System.out.println(nibble2.toHexa());
        }
        catch (Exception exception) {
            exception.printStackTrace();
        }
    }

    static int toBPM(int n) {
        return 60000 / n;
    }

    static int toMillis(int n) {
        return 60000 / n;
    }

    protected void log(String string) {
        if (this.parent != null) {
            this.parent.log(string);
        }
    }

    public void resetValue() {
        this.value = this.getDefaultValue();
    }

    protected int getDefaultValue() {
        int n;
        switch (this.type) {
            case 4: {
                n = -1;
                break;
            }
            case 7: {
                n = 1;
                break;
            }
            case 12: {
                n = 3;
                break;
            }
            case 13: {
                n = 25;
                break;
            }
            case 14: {
                n = 3;
                break;
            }
            case 42: {
                n = -12;
                break;
            }
            case 101: {
                n = -200;
                break;
            }
            case 102: {
                n = -100;
                break;
            }
            case 92: {
                n = -100;
                break;
            }
            case 103: {
                n = -100;
                break;
            }
            case 104: {
                n = -99;
                break;
            }
            case 133: {
                n = -99;
                break;
            }
            case 132: {
                n = -99;
                break;
            }
            case 105: {
                n = -60;
                break;
            }
            case 106: {
                n = -50;
                break;
            }
            case 107: {
                n = -50;
                break;
            }
            case 108: {
                n = -30;
                break;
            }
            case 131: {
                n = -6;
                break;
            }
            case 109: {
                n = -25;
                break;
            }
            case 110: {
                n = -12;
                break;
            }
            case 120: {
                n = 1;
                break;
            }
            case 121: {
                n = 1;
                break;
            }
            case 122: {
                n = 1;
                break;
            }
            case 123: {
                n = 100;
                break;
            }
            case 125: {
                n = 3;
                break;
            }
            case 127: {
                n = -2400;
                break;
            }
            case 129: {
                n = -100;
                break;
            }
            case 130: {
                n = 420;
                break;
            }
            default: {
                n = 0;
            }
        }
        return n;
    }
}

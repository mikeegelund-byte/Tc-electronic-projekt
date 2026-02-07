# Git Hooks Installation

## Quick Setup
```powershell
git config core.hooksPath .githooks
```

## Hooks Included

- **pre-commit**: Builds solution + runs unit tests before commit
- **commit-msg**: Enforces conventional commit message format

## Disable Temporarily
```powershell
git commit --no-verify
```

## Conventional Commit Format

Format: `type(scope): description`

**Valid types:**
- `feat` - New feature
- `fix` - Bug fix
- `docs` - Documentation only
- `refactor` - Code change (no new feature or fix)
- `test` - Add/update tests
- `chore` - Maintenance tasks
- `perf` - Performance improvements
- `ci` - CI/CD changes
- `build` - Build system changes
- `revert` - Revert previous commit

**Examples:**
- `feat(midi): add SysEx validation`
- `fix(ui): resolve layout bug in preset editor`
- `docs(readme): update installation instructions`
- `refactor(domain): simplify parameter validation`
- `test(midi): add contract tests for port behavior`

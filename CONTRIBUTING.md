# Contributing to Nova System Manager

## Development Workflow

1. **Branch Naming:**
   - `feature/description` - New features
   - `fix/description` - Bug fixes
   - `docs/description` - Documentation updates

2. **Commit Messages:**
   Follow conventional commits: `type(scope): description`
   - Install git hooks: `git config core.hooksPath .githooks`

3. **Testing:**
   - All new features must have unit tests
   - Run `dotnet test` before committing
   - Maintain 95%+ coverage in Domain layer

4. **Code Style:**
   - Follow .editorconfig settings
   - Enable format-on-save in your editor
   - Use nullable reference types

## Local-First Development

This project uses a local-first workflow:
1. All development happens locally
2. Merge features to main locally after verification
3. Push to backup remote only when needed
4. Tag releases with semantic versioning

## Pull Request Process

Since this is local development:
1. Create feature branch
2. Make changes and commit with conventional format
3. Run tests: `dotnet test`
4. Merge to main locally
5. Optional: Push to backup remote

## Architecture Guidelines

See `docs/DESIGN/DESIGN-Architecture.md` for Clean Architecture patterns.

Key principles:
- Domain layer has no dependencies
- Application layer depends only on Domain
- Infrastructure implements interfaces from Domain
- Presentation depends on Application

## Testing Guidelines

See `docs/PROCESS/PROC-TestingStrategy.md` for testing approach.

- Unit tests for all business logic
- Contract tests for interfaces
- Mock MIDI hardware for unit tests
- Separate hardware tests in Nova.HardwareTest project

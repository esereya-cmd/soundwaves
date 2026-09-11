# Contributing to Soundwaves

## Branch Naming

Do not work directly on `main`.

Use a separate branch for each task.

Examples:
- feature/frontend-shell
- feature/database-schema
- fix/login-error
- docs/setup-guide

## Commit Workflow

Before starting work:

git checkout main
git pull origin main

Then create a new branch:

git checkout -b branch-name

After making changes:

git add .
git commit -m "Short description"
git push -u origin branch-name

## Pull Request Process

1. Push the branch to GitHub.
2. Create a Pull Request into `main`.
3. Request review from at least one teammate.
4. Make changes if requested.
5. Merge only after approval.

## Main Branch Rule

Do not use `main` as a personal working branch.

All changes should follow:

Branch -> Pull Request -> Review -> Merge
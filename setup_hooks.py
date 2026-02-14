#!/usr/bin/env python3
"""
Setup git pre-commit hooks for the mod workspace.

Usage:
    python setup_hooks.py install    # Install pre-commit hooks
    python setup_hooks.py uninstall  # Remove pre-commit hooks
"""

import os
import sys
import stat
from pathlib import Path

WORKSPACE_ROOT = Path(__file__).parent.resolve()
GIT_HOOKS_DIR = WORKSPACE_ROOT / ".git" / "hooks"
PRE_COMMIT_HOOK = GIT_HOOKS_DIR / "pre-commit"

PRE_COMMIT_SCRIPT = """#!/usr/bin/env python3
\"\"\"
Pre-commit hook for Erenshor mods.
Runs health checks and optionally builds mods before committing.
\"\"\"

import sys
import subprocess
from pathlib import Path

WORKSPACE_ROOT = Path(__file__).parent.parent.parent

def run_doctor():
    \"\"\"Run health checks on all mods.\"\"\"
    print("Running mod health checks...")
    result = subprocess.run(
        [sys.executable, str(WORKSPACE_ROOT / "doctor.py")],
        cwd=WORKSPACE_ROOT
    )
    return result.returncode == 0

def run_build():
    \"\"\"Build all mods.\"\"\"
    print("Building all mods...")
    result = subprocess.run(
        [sys.executable, str(WORKSPACE_ROOT / "build.py")],
        cwd=WORKSPACE_ROOT
    )
    return result.returncode == 0

def main():
    print("=" * 60)
    print("Pre-commit hook: Erenshor Mods")
    print("=" * 60)
    
    # Run health checks
    if not run_doctor():
        print("\\n✗ Health checks failed!")
        print("Fix the issues or use 'git commit --no-verify' to skip checks.")
        return 1
    
    print("\\n✓ Health checks passed!")
    
    # Optionally build mods (commented out by default)
    # Uncomment the following lines to build before committing:
    # if not run_build():
    #     print("\\n✗ Build failed!")
    #     print("Fix build errors or use 'git commit --no-verify' to skip.")
    #     return 1
    # print("\\n✓ Build successful!")
    
    print("\\n✓ Pre-commit checks passed!")
    return 0

if __name__ == "__main__":
    sys.exit(main())
"""


def install_hooks():
    """Install pre-commit hooks."""
    if not GIT_HOOKS_DIR.exists():
        print("Error: .git/hooks directory not found. Is this a git repository?")
        return False
    
    # Write pre-commit hook
    PRE_COMMIT_HOOK.write_text(PRE_COMMIT_SCRIPT)
    
    # Make executable
    PRE_COMMIT_HOOK.chmod(PRE_COMMIT_HOOK.stat().st_mode | stat.S_IEXEC)
    
    print(f"✓ Installed pre-commit hook at {PRE_COMMIT_HOOK}")
    print("\nThe hook will:")
    print("  - Run health checks (doctor.py) before each commit")
    print("  - Optionally build mods (commented out by default)")
    print("\nTo skip the hook, use: git commit --no-verify")
    
    return True


def uninstall_hooks():
    """Remove pre-commit hooks."""
    if PRE_COMMIT_HOOK.exists():
        PRE_COMMIT_HOOK.unlink()
        print(f"✓ Removed pre-commit hook from {PRE_COMMIT_HOOK}")
        return True
    else:
        print("No pre-commit hook found.")
        return False


def main():
    import argparse
    
    parser = argparse.ArgumentParser(description="Setup git pre-commit hooks")
    parser.add_argument("action", choices=["install", "uninstall"], 
                       help="Install or uninstall hooks")
    
    args = parser.parse_args()
    
    if args.action == "install":
        return 0 if install_hooks() else 1
    elif args.action == "uninstall":
        return 0 if uninstall_hooks() else 1
    
    return 0


if __name__ == "__main__":
    sys.exit(main())


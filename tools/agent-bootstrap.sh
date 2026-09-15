#!/bin/bash
set -e

# Agent Bootstrap Script — Competition Management System
# 
# Purpose:
#   Prepare a fresh checkout to run the test suite and validation tools.
#   Do this once after cloning the repo or when dependencies change.
#
# Usage:
#   ./tools/agent-bootstrap.sh
#
# What it does:
#   1. Checks that dotnet is installed
#   2. Restores NuGet dependencies
#   3. Builds the solution
#   4. Verifies Python3 is available (for architecture validation)
#   5. Prints readiness status
#
# After bootstrap:
#   - dotnet test                                    (run all tests)
#   - dotnet build                                   (rebuild)
#   - python3 scripts/validate_boundaries.py ...    (check architecture)

echo "🚀 Bootstrapping Competition Management System..."
echo ""

# Check dotnet
echo "✓ Checking dotnet CLI..."
if ! command -v dotnet &> /dev/null; then
    echo "❌ dotnet CLI not found. Install from https://dotnet.microsoft.com/download"
    exit 1
fi
DOTNET_VERSION=$(dotnet --version)
echo "  Found dotnet $DOTNET_VERSION"
echo ""

# Check Python3 (for architecture validator)
echo "✓ Checking Python3..."
if ! command -v python3 &> /dev/null; then
    echo "❌ Python3 not found. Install from https://www.python.org/downloads/ or use homebrew: brew install python3"
    exit 1
fi
PYTHON_VERSION=$(python3 --version)
echo "  Found $PYTHON_VERSION"
echo ""

# Restore dependencies
echo "✓ Restoring NuGet dependencies..."
dotnet restore
echo ""

# Build the solution
echo "✓ Building solution..."
dotnet build
echo ""

# Verify test framework
echo "✓ Checking test framework..."
if [ ! -f "tests/CompetitionManager.Tests/CompetitionManager.Tests.csproj" ]; then
    echo "❌ Test project not found at tests/CompetitionManager.Tests/CompetitionManager.Tests.csproj"
    exit 1
fi
echo "  Test project found"
echo ""

# Quick test run
echo "✓ Running a quick test to verify setup..."
dotnet test --no-build --verbosity=quiet || echo "  (No tests written yet; this is OK)"
echo ""

# Architecture validation
echo "✓ Validating architecture..."
python3 scripts/validate_boundaries.py architecture.json --repo-root . || {
    echo "⚠️  Architecture validation failed (may be normal if files haven't been created yet)"
}
echo ""

# Success
echo "✅ Bootstrap complete!"
echo ""
echo "Next steps:"
echo "  1. Read AGENTS.md for layer boundaries and conventions"
echo "  2. Read docs/agents/workflow-guide.md for the workflow"
echo "  3. Start with: /grill-me"
echo ""
echo "Quick commands:"
echo "  dotnet build                     # Rebuild"
echo "  dotnet test                      # Run all tests"
echo "  python3 scripts/validate_boundaries.py architecture.json --repo-root .  # Validate architecture"
echo ""

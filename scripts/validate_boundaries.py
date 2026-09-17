#!/usr/bin/env python3
"""
Architecture boundary validator for Competition Management System.

Validates that the codebase respects the layer boundaries and dependency rules
defined in architecture.json.

Usage:
    python3 scripts/validate_boundaries.py architecture.json --repo-root .
    python3 scripts/validate_boundaries.py architecture.json --repo-root . --file src/CompetitionManager.Api/Controllers/CompetitionsController.cs
"""

import json
import sys
import argparse
from pathlib import Path
from typing import List, Dict, Set, Tuple
import re


class ArchitectureValidator:
    def __init__(self, architecture_path: str, repo_root: str = "."):
        self.repo_root = Path(repo_root)
        with open(architecture_path, "r") as f:
            self.architecture = json.load(f)
        self.violations: List[str] = []
        self.warnings: List[str] = []
        self.layer_map = self._build_layer_map()

    def _build_layer_map(self) -> Dict[str, Dict]:
        """Map layer IDs to layer definitions."""
        return {layer["id"]: layer for layer in self.architecture["layers"]}

    def _get_layer_for_file(self, file_path: Path) -> str:
        """Determine which layer a file belongs to."""
        rel_path = file_path.relative_to(self.repo_root)
        for layer_id, layer_def in self.layer_map.items():
            # Simple glob matching (not full glob, just prefix matching for now)
            pattern = layer_def["path"].replace("**", "*")
            if self._matches_pattern(str(rel_path), pattern):
                return layer_id
        return None

    def _matches_pattern(self, path: str, pattern: str) -> bool:
        """Check if path matches a simple pattern."""
        # Convert pattern to regex
        regex_pattern = pattern.replace(".", r"\.").replace("*", ".*")
        return re.match(f"^{regex_pattern}$", path) is not None

    def _extract_imports(self, file_content: str, file_path: Path) -> List[str]:
        """Extract using statements from a C# file."""
        imports = []
        # Match: using Namespace;
        pattern = r"using\s+([A-Za-z0-9_.]+)\s*;"
        matches = re.findall(pattern, file_content)
        return matches

    def _namespace_to_layer(self, namespace: str) -> str:
        """Map a namespace to a layer ID."""
        if namespace.startswith("CompetitionManager.Api"):
            return "api"
        elif namespace.startswith("CompetitionManager.Application"):
            return "application"
        elif namespace.startswith("CompetitionManager.Domain"):
            return "domain"
        elif namespace.startswith("CompetitionManager.Infrastructure"):
            return "infrastructure"
        return None

    def validate_file(self, file_path: Path) -> List[Tuple[str, str]]:
        """Validate a single C# file for boundary violations."""
        violations = []

        if not file_path.suffix == ".cs":
            return violations

        try:
            with open(file_path, "r", encoding="utf-8") as f:
                content = f.read()
        except Exception as e:
            self.warnings.append(f"Could not read {file_path}: {e}")
            return violations

        source_layer = self._get_layer_for_file(file_path)
        if not source_layer:
            return violations  # File not in any defined layer

        imports = self._extract_imports(content, file_path)
        source_layer_def = self.layer_map[source_layer]
        allowed_deps = source_layer_def.get("dependencies", [])
        forbidden_deps = source_layer_def.get("forbidden_dependencies", [])

        for import_namespace in imports:
            import_layer = self._namespace_to_layer(import_namespace)
            if not import_layer:
                continue

            # The API composition root is the one allowed place to wire Infrastructure implementations.
            if (
                source_layer == "api"
                and file_path.name == "Program.cs"
                and import_layer == "infrastructure"
            ):
                continue

            # Check if import is allowed
            if import_layer == source_layer:
                continue  # Same layer is always OK

            if import_layer not in allowed_deps:
                violations.append((
                    file_path,
                    f"Layer '{source_layer}' cannot depend on '{import_layer}' (via {import_namespace}). "
                    f"Allowed dependencies: {', '.join(allowed_deps)}"
                ))
            elif import_layer in forbidden_deps:
                violations.append((
                    file_path,
                    f"Layer '{source_layer}' explicitly forbidden from depending on '{import_layer}' (via {import_namespace})"
                ))

        return violations

    def validate_all(self) -> bool:
        """Validate entire codebase."""
        cs_files = list(self.repo_root.glob("src/**/*.cs"))

        for file_path in cs_files:
            violations = self.validate_file(file_path)
            for viol_file, viol_msg in violations:
                self.violations.append(f"{viol_file}: {viol_msg}")

        return len(self.violations) == 0

    def validate_specific_file(self, file_pattern: str) -> bool:
        """Validate a specific file or pattern."""
        file_path = self.repo_root / file_pattern
        if not file_path.exists():
            self.warnings.append(f"File not found: {file_path}")
            return True

        violations = self.validate_file(file_path)
        for viol_file, viol_msg in violations:
            self.violations.append(f"{viol_file}: {viol_msg}")

        return len(self.violations) == 0

    def report(self):
        """Print validation report."""
        if self.violations:
            print("\n❌ ARCHITECTURE VIOLATIONS DETECTED:\n", file=sys.stderr)
            for violation in self.violations:
                print(f"  ❌ {violation}", file=sys.stderr)
            print(f"\n  Total violations: {len(self.violations)}\n", file=sys.stderr)
        else:
            print("\n✓ Architecture boundaries are valid.\n", file=sys.stdout)

        if self.warnings:
            print("⚠ WARNINGS:\n", file=sys.stderr)
            for warning in self.warnings:
                print(f"  ⚠ {warning}", file=sys.stderr)

    def exit_code(self) -> int:
        """Return exit code: 0 if valid, 1 if violations found."""
        return 1 if self.violations else 0


def main():
    parser = argparse.ArgumentParser(
        description="Validate architecture boundaries in Competition Management System"
    )
    parser.add_argument(
        "architecture_file",
        help="Path to architecture.json"
    )
    parser.add_argument(
        "--repo-root",
        default=".",
        help="Root directory of the repository (default: current directory)"
    )
    parser.add_argument(
        "--file",
        help="Validate a specific file (optional)"
    )

    args = parser.parse_args()

    validator = ArchitectureValidator(args.architecture_file, args.repo_root)

    if args.file:
        success = validator.validate_specific_file(args.file)
    else:
        success = validator.validate_all()

    validator.report()
    sys.exit(validator.exit_code())


if __name__ == "__main__":
    main()

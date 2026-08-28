# Repository Agent Harness

This directory contains a lightweight repository-level harness for
AI-assisted software development.

It is intentionally simple.

The application does not depend on this directory at runtime.

## Goals

The harness exists to:

- make agent responsibilities explicit
- provide reusable engineering procedures
- reduce context drift between AI sessions
- separate implementation from independent verification
- keep AI collaboration portable between coding assistants
- leave auditable evidence of important engineering decisions

## Structure


.agents/
├── agents/
│   ├── architect.md
│   ├── implementer.md
│   ├── reviewer.md
│   └── challenge-auditor.md
├── skills/
│   ├── plan/
│   ├── implement/
│   ├── review/
│   └── challenge-audit/
└── plans/



## Agents vs Skills

Agents define responsibility and authority.

Skills define reusable procedures.

Example:

Reviewer
   +
review skill
   ↓
independent engineering review
Portability

The canonical definitions live here rather than inside a vendor-specific
directory such as .claude/, .codex/, or .cursor/.

Tools that support repository instructions can read AGENTS.md.

Tools that support skills or custom agents may expose the files in this
directory through their native mechanism.

If a coding assistant has no native skill support, the same procedure can
still be executed by directly reading the relevant SKILL.md.

Non-goals

This is not:

an autonomous agent platform
a workflow server
a multi-agent runtime
an LLM abstraction layer
part of the production application

It is engineering guidance and process automation stored alongside the code.
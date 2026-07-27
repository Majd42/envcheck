# envcheck

A small CLI that catches a common source of "works on my machine" bugs: an
out-of-date `.env` file. `envcheck` compares your `.env` against a
`.env.example` template and tells you exactly which keys are missing, blank,
or extra — before your app crashes at startup with a confusing error.

## Install

```bash
dotnet tool install --global envcheck
```

Requires the [.NET SDK](https://dotnet.microsoft.com/download) (net10.0 or later).

## Usage

```bash
envcheck
```

By default this compares `.env.example` against `.env` in the current
directory. Both paths can be customized:

```bash
envcheck --example config/.env.example --env config/.env
```

### Options

| Flag | Description |
| --- | --- |
| `--example <path>` | Path to the template file (default: `.env.example`) |
| `--env <path>` | Path to the file to validate (default: `.env`) |
| `--strict` | Also fail when `.env` has keys not present in the example |
| `--fix` | Append missing keys to `.env`, using the values from `.env.example` |
| `--json` | Output the comparison result as JSON (machine-readable) |
| `-v`, `--version` | Show version information |
| `-h`, `--help` | Show help |

### Exit codes

| Code | Meaning |
| --- | --- |
| `0` | `.env` matches the example |
| `1` | `.env` is missing keys, has empty values, or (with `--strict`) has extra keys |
| `2` | Usage error, e.g. a file could not be found |

The exit code makes `envcheck` easy to drop into a pre-commit hook or CI
pipeline:

```yaml
- name: Check .env matches .env.example
  run: envcheck --example .env.example --env .env
```

## Example

Given:

```dotenv
# .env.example
DATABASE_URL=
API_KEY=
DEBUG=
```

```dotenv
# .env
DATABASE_URL=postgres://localhost/dev
API_KEY=
```

Running `envcheck` reports:

```
Missing keys (present in .env.example, missing from .env):
  - DEBUG
Empty values (present in .env but blank):
  - API_KEY
```

and exits with code `1`.

### Auto-fixing missing keys

Run with `--fix` to append any missing keys straight into `.env`, copying
their default value from `.env.example`:

```bash
envcheck --fix
```

```
Added 1 missing key(s) to .env:
  - DEBUG

Empty values (present in .env but blank):
  - API_KEY
```

`--fix` only adds keys that are completely missing — it never overwrites
existing values, so blank values (like `API_KEY` above) are left for you to
fill in yourself.

### JSON output

For scripting and CI, `--json` emits the full result as a machine-readable
object instead of the coloured text report. The exit code is unchanged, so
you can still branch on success/failure:

```bash
envcheck --json
```

```json
{
  "example": ".env.example",
  "env": ".env",
  "ok": false,
  "missingKeys": [
    "DEBUG"
  ],
  "emptyValueKeys": [
    "API_KEY"
  ],
  "extraKeys": [],
  "addedKeys": []
}
```

When combined with `--fix`, any keys that were appended appear under
`addedKeys`.

## Building from source

```bash
git clone https://github.com/Majd42/envcheck.git
cd envcheck
dotnet build
dotnet test
```

## License

[MIT](LICENSE)

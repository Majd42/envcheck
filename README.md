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

## Building from source

```bash
git clone https://github.com/Majd42/envcheck.git
cd envcheck
dotnet build
dotnet test
```

## License

[MIT](LICENSE)

# Environments (SSOT: app/Content/config/environments.json)

| | Staging | Production |
|---|---|---|
| App URL | http://127.0.0.1:5181 | http://127.0.0.1:5182 |
| ASPNETCORE_ENVIRONMENT | Staging | Production |
| Mongo port / db | 27018 / heritagetrail_staging | 27019 / heritagetrail_prod |
| Mongo dbpath | artifacts/mongo/staging | artifacts/mongo/prod |
| Uploads | artifacts/uploads/staging | artifacts/uploads/prod |
| Mail outbox | artifacts/mail/staging | artifacts/mail/prod |
| Seed mode | Reset (drops + re-seeds every start) | Once (seeds only when empty) |
| Env badge | "STAGING — mock data" ribbon | none |
| Atoms gallery /atoms | enabled | 404 |
| Logs | artifacts/logs/staging-*.log | artifacts/logs/prod-*.log |

Same code, same tokens, same atoms. Only config + data differ → proof: edit a staging draft, prod stays unchanged; staging badge visible only on 5181.

Run:
- `powershell -ExecutionPolicy Bypass -File scripts/staging.ps1`
- `powershell -ExecutionPolicy Bypass -File scripts/prod.ps1`
- health: `GET /healthz` → `{status, environment, db, badge}`

Prod starts only after the user signs off staging (page-level screenshots in chat).

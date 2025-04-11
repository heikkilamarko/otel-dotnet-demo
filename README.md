# OTEL Demo

## Create Azure Resources

```bash
terraform -chdir=infra init
```

```bash
terraform -chdir=infra apply
```

## Setup Environment Variables

```bash
./setup_env.sh
```

## Start Services

```bash
docker compose up --build -d
```

## Make Requests

```bash
curl -i http://localhost:8080/rolldice/demo
```

## Stop Services

```bash
docker compose down
```

## Destroy Azure Resources

```bash
terraform -chdir=infra destroy
```

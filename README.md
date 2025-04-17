# OTEL Demo

![components](doc/components.png)

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
curl -i -X POST http://localhost:8080/rolldice/demo-player
```

## Stop Services

```bash
docker compose down
```

## Destroy Azure Resources

```bash
terraform -chdir=infra destroy
```

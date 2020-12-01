# surferjeff-hello-probot

> A GitHub App built with [Probot](https://github.com/probot/probot) that My first probot app

## Setup

```sh
# Install dependencies
npm install

# Compile
npm run build

# Run
npm run start
```

## Docker

```sh
# 1. Build container
docker build -t surferjeff-hello-probot .

# 2. Start container
docker run -e APP_ID=<app-id> -e PRIVATE_KEY=<pem-value> surferjeff-hello-probot
```

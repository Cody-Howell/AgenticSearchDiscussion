set -e

docker compose down -v

cd agent-react

pnpm run build

cd ..

docker  compose up -d --build
FROM node:18 AS base

FROM base AS dependencies

WORKDIR /app
COPY package*.json ./
RUN yarn install --frozen-lockfile

FROM base AS build

WORKDIR /app
COPY --from=dependencies /app/node_modules ./node_modules
COPY . .

RUN yarn run build

FROM base AS deploy
WORKDIR /app

COPY --from=dependencies /app/node_modules ./node_modules
COPY --from=build /app/package*.json .
COPY --from=build /app/public ./public

COPY --from=build /app/.next/ ./.next/

EXPOSE 3000
ENV PORT 3000

CMD ["yarn", "run", "start"]

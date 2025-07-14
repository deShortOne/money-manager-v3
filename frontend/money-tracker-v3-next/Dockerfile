FROM node:18 AS base

FROM base AS dependencies

WORKDIR /app
COPY package*.json ./
RUN yarn install --frozen-lockfile

FROM base AS build

WORKDIR /app
COPY --from=dependencies /app/node_modules ./node_modules
COPY . .


EXPOSE 3000
ENV PORT 3000

CMD ["yarn", "run", "dev"]

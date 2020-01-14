FROM node:11.6.0-alpine AS builder
WORKDIR /src
COPY ["Web/frontend/", "frontend/"]
WORKDIR "/src/frontend"
RUN npm install
RUN npx ng build --prod --env=docker

FROM nginx:1.15.8-alpine
COPY --from=builder /src/frontend/dist/ /usr/share/nginx/html

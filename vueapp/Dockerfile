# Шаг 1: Билд Vue приложения
FROM node:20-alpine AS build-stage
WORKDIR /app
COPY . .
COPY .env .env
RUN npm install
RUN npm run build

# Шаг 2: Сервер Nginx
FROM nginx:alpine
COPY --from=build-stage /app/dist /usr/share/nginx/html
COPY ./nginx.conf /etc/nginx/nginx.conf

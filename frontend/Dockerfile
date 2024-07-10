FROM node:18

WORKDIR /usr/src/app

EXPOSE 8080

ENTRYPOINT ["/bin/sh", "-c", "npm install && npm run build-only && npm run dev"]

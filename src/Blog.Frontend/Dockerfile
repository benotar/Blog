FROM node:20.17.0-alpine
WORKDIR /app-frontend

ARG VITE_API_URL_ARG
ARG VITE_FIREBASE_API_KEY_ARG

ENV VITE_API_URL=$VITE_API_URL_ARG
ENV VITE_FIREBASE_API_KEY=$VITE_FIREBASE_API_KEY_ARG

COPY package*.json ./
RUN npm install
COPY . ./
CMD ["npm", "run", "dev"]
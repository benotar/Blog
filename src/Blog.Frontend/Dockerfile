FROM node:20.17.0-alpine
WORKDIR /app-frontend
COPY package*.json ./
RUN npm install
COPY . ./
CMD ["npm", "run", "dev"]
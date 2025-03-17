# Use a lightweight Node.js image for building
FROM node:18-alpine AS build

# Set the working directory inside the container
WORKDIR /app

# Copy package.json and package-lock.json first (to cache dependencies)
COPY package*.json ./

# Install dependencies
RUN npm install

# Copy all frontend source files
COPY . . 

# Inject environment variables at build time
ARG REACT_APP_API_URL
ENV REACT_APP_API_URL=${REACT_APP_API_URL}

# Build the React app
RUN npm run build

# Use Nginx to serve the React build
FROM nginx:alpine AS serve
WORKDIR /usr/share/nginx/html

# Remove default Nginx static assets and replace with our React build
RUN rm -rf ./*
COPY --from=build /app/build ./

# Expose port 3000
EXPOSE 3000

# Start Nginx
CMD ["nginx", "-g", "daemon off;"]

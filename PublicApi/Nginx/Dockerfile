FROM nginx:1.25.3
ADD conf.d /etc/nginx/conf.d

# Health check to verify container is running OK
RUN DEBIAN_FRONTEND=noninteractive apt-get update && apt-get install -yq curl
HEALTHCHECK --interval=10s --timeout=5s --start-period=30s \
  CMD curl -f http://localhost/nginx-health || exit 1

# 禅道

### DB

```
create database zentao;
CREATE USER 'zentao'@'%' IDENTIFIED BY 'zentao';
GRANT ALL PRIVILEGES ON zentao.* TO 'zentao'@'%';
FLUSH PRIVILEGES;
```

### Docker Compose

```
services:
  zentao:
    image: hub.zentao.net/app/zentao:20.1.0
    container_name: zentao
    ports:
      - '8090:80'
    volumes:
      - /mnt/shared/zentao:/data
    environment:
      - ZT_MYSQL_HOST=host.docker.internal
      - ZT_MYSQL_PORT=3306
      - ZT_MYSQL_USER=zentao
      - ZT_MYSQL_PASSWORD=zentao
      - ZT_MYSQL_DB=zentao
      - PHP_MAX_EXECUTION_TIME=120
      - PHP_MEMORY_LIMIT=512M
      - PHP_POST_MAX_SIZE=128M
      - PHP_UPLOAD_MAX_FILESIZE=128M
      - LDAP_ENABLED=false
      - SMTP_ENABLED=false
      - APP_DEFAULT_PORT=80
      - APP_DOMAIN=zentao.y.home
      - PROTOCOL_TYPE=http
      - IS_CONTAINER=true
      - LINK_GIT=false
      - LINK_CI=false
    extra_hosts:
      - "host.docker.internal:host-gateway"
    restart: unless-stopped
```
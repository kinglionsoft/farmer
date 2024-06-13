# NextCloud

## 安装

1. 创建共享文件夹

2. mariadb

```
create database nextcloud;
CREATE USER 'nextcloud'@'%' IDENTIFIED BY 'nextcloud';
GRANT ALL PRIVILEGES ON nextcloud.* TO 'nextcloud'@'%';
FLUSH PRIVILEGES;

```

3. docker

```
docker run -d --name nextcloud \
       -p 8088:80 \
       -e PUID=1000 \
       -e PGID=100 \
       --add-host=host.docker.internal:host-gateway \
       --restart=unless-stopped \
       -v /mnt/shared/nextcloud/html:/var/www/html \
       -v /mnt/shared/nextcloud/data:/var/www/html/data \
              
```

4. docker compose

```
---
# https://hub.docker.com/r/linuxserver/nextcloud
# https://docs.linuxserver.io/images/docker-nextcloud
# only x86-64 and arm64 are supported
services:
  nextcloud:
    image: nextcloud:latest
    container_name: nextcloud
    environment:
      - PUID=1000
      - PGID=1000
      - TZ=Asia/Shanghai
    volumes:
      - /mnt/shared/nextcloud/html:/var/www/html
      - /mnt/shared/nextcloud/data:/var/www/html/data
    ports:
      - 8088:80
    extra_hosts:
      - "host.docker.internal:host-gateway"
    restart: unless-stopped
```

5. nginx

```

```
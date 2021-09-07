proxy_cache_path /mnt/data/nginx/cache levels=1:2 keys_zone=cmm_oss:100m inactive=7d max_size=100g;
proxy_temp_path /mnt/data/nginx/temp 1 2;

server {
    listen 80;

    client_max_body_size 50m;

    location / {
        proxy_pass http://xx;
        proxy_cache cmm_oss;
        proxy_redirect off;
        proxy_cache_valid 200 302 30d;
        proxy_cache_valid 301 1d;
        proxy_cache_valid any 1m;
        proxy_cache_lock on;
        proxy_cache_lock_timeout 50s;
        add_header  Nginx-Cache "$upstream_cache_status";
        expires 30d;

        proxy_hide_header "Set-Cookie";
        proxy_ignore_headers "Set-Cookie";
        proxy_hide_header "Content-Disposition";

        proxy_set_header X-Forwarded-Host $host:$server_port;
        proxy_set_header X-Forwarded-Server $host;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header Host $host;
   }

}

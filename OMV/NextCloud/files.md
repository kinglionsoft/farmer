# Files

## 手动同步文件数据库

``` bash
## 33 是用户ID
docker exec -u 33 nextcloud  php occ files:scan --all 
```
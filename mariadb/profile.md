# 查询优化

## 查询优化

### 查看索引大小

```
select database_name, table_name, index_name, stat_value*@@innodb_page_size/1024/1024 length_m
from mysql.innodb_index_stats where stat_name='size'
and database_name='db'
AND `table_name`='table'
```

### sum耗时

```sql
-- 开启
SET profiling=1;

-- 执行某个查询

-- 查看查询分析
show PROFILES;
-- 查看 指定分析
show profile cpu,block io,memory,swaps,context switches,source for QUERY 34;

-- 发现 Sending data 耗时巨大

```

### 查看最近死锁

```sql
SHOW ENGINE INNODB STATUS
```
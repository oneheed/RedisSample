docker run --name myredis -p 6379:6379 redis
docker run --volume D:/redis/redis.conf:/usr/local/etc/redis/redis.conf -p 6379:6379 --name myredis redis redis-server /usr/local/etc/redis/redis.conf


docker run --volume D:/redis/redis.conf:/usr/local/etc/redis/redis.conf --name redis-lab -p 6379:6379 -d redis redis-server /usr/local/etc/redis/redis.conf


touch /var/log/redis.log

chmod 777 /var/log/redis.log
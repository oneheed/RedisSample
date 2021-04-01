# Redis Key Command

## <span style="color:#0066aa">COPY</span>

Return value

- 1 if source was copied.
- 0 if source was not copied.

```cmd
127.0.0.1:6379> SET dolly "sheep"
OK
127.0.0.1:6379> COPY dolly clone
(integer) 1
127.0.0.1:6379> GET clone
"sheep"
127.0.0.1:6379> COPY dolly clone <== key exists
(integer) 0
```

## <span style="color:#0066aa">DEL</span> key [key...]

```cmd
127.0.0.1:6379> SET key1 "Hello"
OK
127.0.0.1:6379> SET key2 "World"
OK
127.0.0.1:6379> DEL key1 key2 key3
(integer) 2
```

Return value

- The number of keys that were removed.

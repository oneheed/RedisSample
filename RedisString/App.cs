using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Redis.Core;
using StackExchange.Redis;

namespace RedisString
{
    class App : BaseConnectionMultiplexer
    {
        public App(
            ILoggerFactory factory,
            IHostApplicationLifetime appLifetime,
            IConnectionMultiplexer connection) : base(factory.CreateLogger<App>(), appLifetime, connection)
        {
        }

        public override async Task ExcuteFunc(CancellationToken cancellationToken)
        {
            var stringKey = "stringKey";

            var isDelete = this._cache.KeyDelete(stringKey);

            // 1. 新增一筆String, 回傳是否成功失敗
            var isSet = this._cache.StringSet(stringKey, $"hello {stringKey}");

            // 2. 取的一筆String, 回傳String結果(hello stringKey)
            var stringResult = this._cache.StringGet(stringKey);

            // 3. 回傳String長度
            var stringLength1 = this._cache.StringLength(stringKey);
            var stringLength2 = await this._cache.StringLengthAsync(stringKey);

            // 4. 在原本Key(hello stringKey), append string(hello stringKey append), 回傳append後的長度
            var appendCount = this._cache.StringAppend(stringKey, " append");

            var setList = new KeyValuePair<RedisKey, RedisValue>[]
            {
               new KeyValuePair<RedisKey, RedisValue> ( $"{stringKey}List1",  $"{stringKey}List1" ),
               new KeyValuePair<RedisKey, RedisValue> ( $"{stringKey}List2",  $"{stringKey}List2" ),
               new KeyValuePair<RedisKey, RedisValue> ( $"{stringKey}List3",  $"{stringKey}List3" ),
            };
            this._cache.KeyDelete(setList.Select(x => x.Key).ToArray());
            // 5. 新增多筆String, 回傳是否成功失敗(true/false)
            var isSetList = this._cache.StringSet(setList);


            this._cache.KeyDelete($"{stringKey}2");
            this._cache.StringSet($"{stringKey}2", "test");
            // 6. 先回傳取得原本key裡面value(test), 再將之後的值設定進去(hello stringKey)
            var result = this._cache.StringGetSet($"{stringKey}2", $"hello {stringKey}");
            // 7. 取得key裡面value(hello stringKey), 開始跟結束的String (stringKey)
            var getRange = this._cache.StringGetRange($"{stringKey}2", 6, 14);

            this._cache.KeyDelete($"{stringKey}3");
            this._cache.KeyDelete($"{stringKey}4");
            this._cache.StringSet($"{stringKey}3", $"hello 1234");
            this._cache.StringSet($"{stringKey}4", $"hello 123456");
            // 8. 指定key(hello 1234)裡面 index 的更換成 value (hello test) => 回傳key字串長度
            //    指定key(hello 123456)裡面 index 的更換成 value (hello test56) => 回傳key字串長度
            var result3 = this._cache.StringSetRange($"{stringKey}3", 6, "test");
            var result4 = this._cache.StringSetRange($"{stringKey}4", 6, "test");

            this._cache.KeyDelete($"{stringKey}5");
            this._cache.StringSet($"{stringKey}5", "a"); // 01100001 (97)
            var strbit1 = this._cache.StringGet($"{stringKey}5");

            // 9. 取得 字串 位移的 Bit 值
            var bitGetResult6 = this._cache.StringGetBit($"{stringKey}5", 6); // 0
            var bitGetResult7 = this._cache.StringGetBit($"{stringKey}5", 7); // 1

            // 10. 設定 字串 位移的 Bit 值, 先取得舊字串 位移的 Bit 值
            var bitSetResult6 = this._cache.StringSetBit($"{stringKey}5", 6, true);  // 01100011
            var bitSetResult7 = this._cache.StringSetBit($"{stringKey}5", 7, false); // 01100010
            var strbit2 = this._cache.StringGet($"{stringKey}5"); // 01100010 (98) "b"

            this._cache.KeyDelete($"{stringKey}6");
            this._cache.StringSet($"{stringKey}6", "ac"); // 01100001 01100011
            var strbit3 = this._cache.StringGet($"{stringKey}6");

            // 11. 取得 key 裡面字串 Bit(1) 個數
            var bitCount1 = this._cache.StringBitCount($"{stringKey}6", 0, -1); // 01100001 01100011 => 7
            var bitCount2 = this._cache.StringBitCount($"{stringKey}6", 0, -2); // 01100001 => 3
            var bitCount3 = this._cache.StringBitCount($"{stringKey}6", 1, -1); // 01100011 => 4

            this._cache.KeyDelete($"{stringKey}7");
            this._cache.StringSet($"{stringKey}7", "ac"); // 01100001 01100011
            var strbit4 = this._cache.StringGet($"{stringKey}7");

            // 12. 取得 key 裡面字串, 所選取字串第一個 Bit(1) Position
            var bitPosition1 = this._cache.StringBitPosition($"{stringKey}7", true, 0, -1); // 0"1"100001 01100011 => 1
            var bitPosition2 = this._cache.StringBitPosition($"{stringKey}7", true, 1, 1);  // 01100001 0"1"100011 => 9


            this._cache.KeyDelete($"{stringKey}8");
            this._cache.StringSet($"{stringKey}8", "a");  // 01100001 97
            var strOperation1 = this._cache.StringGet($"{stringKey}8");

            this._cache.KeyDelete($"{stringKey}9");
            this._cache.StringSet($"{stringKey}9", "b");  // 01100010 98
            var strOperation2 = this._cache.StringGet($"{stringKey}9");

            this._cache.KeyDelete($"{stringKey}10");
            this._cache.StringSet($"{stringKey}10", "c"); // 01100011 99
            var strOperation3 = this._cache.StringGet($"{stringKey}10");

            this._cache.KeyDelete($"{stringKey}11");
            this._cache.StringSet($"{stringKey}11", "97"); // 00111001 57 00110111 55
            var strOperation4 = this._cache.StringGet($"{stringKey}11");

            // 13. 取得 key 裡面字串 Bit 邏輯判斷 
            var strOperationResult1 = this._cache.StringBitOperation(Bitwise.And, $"{stringKey}Operation1", $"{stringKey}8", $"{stringKey}9"); // 01100000 96
            var strOperationResult2 = this._cache.StringBitOperation(Bitwise.And, $"{stringKey}Operation2", new RedisKey[] { $"{stringKey}8", $"{stringKey}9", $"{stringKey}10" }); // 01100000 96

            var strOperationResult01 = this._cache.StringGet($"{stringKey}Operation1");
            var strOperationResult02 = this._cache.StringGet($"{stringKey}Operation2");

            var strOperationResult3 = this._cache.StringBitOperation(Bitwise.Or, $"{stringKey}Operation3", $"{stringKey}8", $"{stringKey}9"); // 01100011 99
            var strOperationResult4 = this._cache.StringBitOperation(Bitwise.Or, $"{stringKey}Operation4", new RedisKey[] { $"{stringKey}8", $"{stringKey}9", $"{stringKey}10" }); // 01100011 99

            var strOperationResult03 = this._cache.StringGet($"{stringKey}Operation3");
            var strOperationResult04 = this._cache.StringGet($"{stringKey}Operation4");

            var strOperationResult5 = this._cache.StringBitOperation(Bitwise.Xor, $"{stringKey}Operation5", $"{stringKey}8", $"{stringKey}9"); // 00000011 3
            var strOperationResult6 = this._cache.StringBitOperation(Bitwise.Xor, $"{stringKey}Operation6", new RedisKey[] { $"{stringKey}8", $"{stringKey}9", $"{stringKey}10" }); //(8, 9) 00000011 3 =>  (x, 10) [00000011, 01100011] 01100000 96

            var strOperationResult05 = this._cache.StringGet($"{stringKey}Operation5");
            var strOperationResult06 = this._cache.StringGet($"{stringKey}Operation6");

            // Not 只能轉換單一
            var strOperationResult7 = this._cache.StringBitOperation(Bitwise.Not, $"{stringKey}Operation7", $"{stringKey}8", $"{stringKey}9");   // 10011110 158
            var strOperationResult8 = this._cache.StringBitOperation(Bitwise.Not, $"{stringKey}Operation8", new RedisKey[] { $"{stringKey}9" }); // 10011101 157

            var strOperationResult07 = this._cache.StringGet($"{stringKey}Operation7");
            var strOperationResult08 = this._cache.StringGet($"{stringKey}Operation8");

            var strOperationResult9 = this._cache.StringBitOperation(Bitwise.Not, $"{stringKey}Operation9", $"{stringKey}11");   // 11000110 198 11001000 200

            var strOperationResult09 = this._cache.StringGet($"{stringKey}Operation9");


            this._cache.KeyDelete($"{stringKey}Expire");
            this._cache.StringSet($"{stringKey}Expire", $"{stringKey}Expire");
            // 14. 設定Key Expire
            var isSetExpire = this._cache.KeyExpire($"{stringKey}Expire", TimeSpan.FromSeconds(15));
            // 15. 取得Key Expire
            var strExpire = this._cache.StringGetWithExpiry($"{stringKey}Expire");

            // ==================================================================================================
            // 1. 
            var bactch = this._cache.CreateBatch();
            var data = Enumerable.Range(1, 10)
                .Select(x => new KeyValuePair<RedisKey, RedisValue>($"stringKeyBatch{x}", $"stringKeyBatch{x}"));

            var deleteTasks = bactch.KeyDeleteAsync(data.Select(x => x.Key).ToArray());
            //bactch.Execute();

            var setTasks = bactch.StringSetAsync(data.ToArray());
            var expireTasks = data.Select(x => bactch.KeyExpireAsync(x.Key, TimeSpan.FromSeconds(10))).ToList();

            bactch.Execute();

            var test1 = await Task.WhenAll(deleteTasks);
            var test2 = await Task.WhenAll(setTasks);
            var test3 = await Task.WhenAll(expireTasks);

            //return Task.CompletedTask;
        }
    }
}

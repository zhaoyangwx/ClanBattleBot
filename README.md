# ClanBattleBot
PCR B服23年4月新版会战机器人

需搭配go-cqhttp

地址示例 ws://127.0.0.1:18088/

go-cqhttp设置参考：

    servers:
      - ws:
          # 正向WS服务器监听地址
          address: 0.0.0.0:18088
          middlewares:
            <<: *default

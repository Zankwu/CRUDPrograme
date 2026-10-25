# 作业文档：《异步资源过滤器（IAsyncResourceFilter）》

只新建一个文件，不改任何现有代码：

**`E:\Work Place\ConfigurationEg\CRUDPrograme\作业-资源过滤器.md`**（解决方案根目录）

## 设计原则

- 题目全部建立在你现有代码的真实细节上（`HomeController` 的三个 action、`Filters/` 下已有过滤器、`Program.cs` 注释掉的注册代码、`CustomWebApplicationFactory` 测试基建），延续原课程「讲解→练习→批改」的模式
- 5 道题难度递进：纸面概念 → 热身实战 → 核心缓存 → 排坑实验 → 加分测试
- 只出提示和验收标准，不给答案；文末注明「做完发回逐题批改」

## 题目大纲

**题 1（纸面·管道顺序）**：假设给 `GET /` 新挂一个资源过滤器，写出它和现有 `PersonActionFilter`、`ResponseHeaderAsyncActionFilter`、模型绑定、action 的完整执行顺序；再回答：资源过滤器短路时哪些环节被跳过。检验洋葱模型理解。

**题 2（热身·第一个 IAsyncResourceFilter）**：写 `PersonListResourceFilter`（日志 + Stopwatch 计时），用 **ServiceFilter** 挂到 `/country`——要求在 Program.cs 完成 DI 注册，并用日志验证它确实"包住"了已有的 `PersonsListResultFilter`（前半段日志最先、后半段日志最后）。

**题 3（核心·30 秒响应缓存）**：写 `PersonListCacheResourceFilter`，注入 `IMemoryCache`，挂到 `GET /`：
- 缓存 key = Path + QueryString；命中→设 `context.Result` 短路 + 响应头 `X-Cache: HIT`；未命中→`await next()` 后缓存 + `X-Cache: MISS`
- 只缓存成功响应；提示需注册 `AddMemoryCache()`，并要求用一句话说明为什么选 ServiceFilter 而不是 TypeFilter
- 验收：curl/浏览器两次请求对比、30 秒后过期、期间改数据库数据观察"旧值"现象

**题 4（排坑实验·401 会被缓存吗）**：利用 `/` 上已有的 auth 检查（action filter 层）：先请求 `/?auth=1`（200 被缓存），再请求不带 auth 的 `/`，观察你的过滤器后半段拿到什么、是否把 401 也缓存了；修复"不缓存非成功响应"；思考题：缓存命中会跳过 auth 检查——什么场景下这很危险？

**题 5（加分·集成测试）**：用现成的 `CustomWebApplicationFactory` 写 xUnit 测试：两次 `GET /?auth=1` 均返回 200，且第二次响应头为 `X-Cache: HIT`，`dotnet test` 通过。

## 文档结构

每题含「背景（引用你的真实代码）→ 任务要求（编号）→ 提示 → 验收标准」四段；开头附准备工作（建议先 commit、运行方式 `dotnet run` → `http://localhost:4396`）；文末注明提交批改方式。

完成后你直接做作业，做完把代码/日志/截图发回来，我逐题批改。
namespace Pastry

open System
open Akka.Actor
open Akka.FSharp

module Boss = 
    //在这里生成所有node
    //message:initiated:string
    //type TotalHops = {numHops:int}
    //type Deliever = {hopCount:int; key:int; node_id:int}
    //the id of boss is -100
    type Boss(numNodes:int, numRequests:int, system:ActorSystem) = 
        inherit Actor()

        let mutable stoppingThreshold = numNodes * numRequests
        let mutable totalHops = 0

        let whereis(node_id:int) = 
            system.ActorSelection("user/" + string(node_id))

        let rec nodeInit(actorRefList:List<IActorRef>, list:List<int>, nearbyNode:int) = //哪里再掉这个啊，program.fs嘛
            if list.IsEmpty then
                actorRefList
            else
                //Node(node_id:int, nearbyNode:int, system:ActorSystem)    //这里是要放开，还是备注呢？
                let properties = [|box list.Head; box nearbyNode; box system|]
                //不确定
                let actorRefList = List.append actorRefList [system.ActorOf(Props(typedefof<Worker.Node>, properties), name = string(list.Head))]
                nodeInit(actorRefList, list.Tail, list.Head)

        let pastryStart(actorRefList:List<IActorRef>) = 
            List.map (fun x -> x <! {Worker.StartRequest.numRequests = numRequests}) actorRefList

        let pastry() = 
            //generate a set of unique node ids
            printfn "pasry1"
            let nodeidArray = createNodeid.generateUniqueId(numNodes)
            
            //convert the set to list
            let nodeidList = Seq.toList nodeidArray
            
            //Node(node_id:int, nearbyNode:int, system:ActorSystem)
            let properties = [|box nodeidList.[0]; box -1;box system|]
            //let nodeSerialNumber = List.append [system.ActorOf(Props(typedefof<Worker.Node>, properties),name = string(nodeidList.[0]))]
            let firstNode = system.ActorOf(Props(typedefof<Worker.Node>, properties),name = string(nodeidList.[0]))
            //system.ActorOf(Props(typedefof<Worker.Node>, properties),name = string(id)))
            //spawn all the nodes recursively
            let actorRefList = nodeInit(List.empty, nodeidList.Tail, nodeidList.Head)
            //for i=1 to nodeidList.Length-1 do
                //let property = [|box nodeidList.[i];box nodeidList.[i-1] ;box system|]
                //let nodeSerialNumber = List.append [system.ActorOf(Props(typedefof<Worker.Node>, property),name = string(nodeidList.[i]))]
                //nodeSerialNumber <- List.append 
                //这里let不能在最后，可是别的却没啥可写的
            //let actorRefList = nodeInit(List.empty. list.Tail, list.Head)
            //first node: nearbyNode = -1, others: nearbyNode = the previous one
            //let actorRefList = nodeInit(List.empty. list.Tail, list.Head)
            
            //计时部分
            //pastryStart(actorRefList) |> ignore
            let stopWatch = System.Diagnostics.Stopwatch.StartNew()
            pastryStart((List.append actorRefList [firstNode])) |> ignore
            stopWatch.Stop()
            let timediff = stopWatch.Elapsed.TotalMilliseconds       
            timediff

(*        let rec pastryDeliver(boss:int, stoppingThreshold:int, numHops:int) = 
            if stoppingThreshold = 0 then
                let bossNode = whereis(boss)
                let totalhopsmsg = {Worker.TotalHops.numHops = numHops}
                bossNode <! totalhopsmsg
            else
                pastryDeliver(boss, stoppingThreshold - 1, numHops)*)


        override x.OnReceive message = 
            match box message with
            | :? string as msg -> 
                //"start"
                printfn "%s" msg
                let timediff = pastry()
                printfn "Time taken for distributing message: %f ms" timediff
            | :? Worker.Initiated as msg ->
                printfn "Node %d has been created." msg.nodeId
            | :? Worker.TotalHops as msg -> 
                let average = float(msg.numHops) / float(numNodes * numRequests)
                printfn "=============================Average number of hops is %f." average
                system.Terminate() |> ignore
            | :? Worker.Deliever as msg ->
                printfn "Message with key %d was delivered at %d in %d hops." msg.key msg.node_id msg.hopCount
                if stoppingThreshold = 0 then
                    //boss
                    whereis(-100) <! {Worker.TotalHops.numHops = totalHops}//这里是不是有问题  应该传给-100？
                else
                    totalHops <- totalHops + msg.hopCount
                    printfn "============stoppingThreshold %d" stoppingThreshold
                    stoppingThreshold <- stoppingThreshold - 1
                    if stoppingThreshold =0 then
                        whereis(-100) <! {Worker.Deliever.hopCount = 0; Worker.Deliever.key = -1; Worker.Deliever.node_id = -1}
            | _ -> x.Unhandled message


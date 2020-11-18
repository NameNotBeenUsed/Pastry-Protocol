namespace Pastry

open System
open Akka
open Akka.Actor
open Akka.FSharp

open FSharpx.Collections
//open Pastry

module createNodeid = 
    let randomizer(length:int) :string= //generate one nodeid，return oneNodeIdstr:string to store this nodeid/
        let mutable ondeNodeId: string = "abcdefgh"
        let randomCh = 
            let baselist = "01234"
            let random = Random()
            fun () -> baselist.Chars (random.Next (baselist.Length - 1))//random from 0-3

        let rndomChtemp = 
            let baselisttemp = "1234"
            let random = Random()
            fun () -> baselisttemp.Chars (random.Next (baselisttemp.Length - 1))
        let array = ondeNodeId.ToCharArray()
        array.[0]<- rndomChtemp()
        for i=1 to length-1 do
            array.[i] <- randomCh()
        let oneNodeIdstr = new string(array)
        //oneNodeIdstr |> printfn "%s"  
        oneNodeIdstr
    
    let generateUniqueId(numNodes:int) =  //generate nodeids which have no repeting element *return a type of resizeArray<string> should open FSharpx.Collections*
        
        let mutable tempList = ResizeArray<int>()
        
        //let mutable tempList = ResizeArray.distinct(nodeidList) 
        while ResizeArray.length(tempList) < (numNodes) do
            
            //let x = tempList.[1]  use this method to get the n.th value of the type of ResizeArray<string>
            tempList.Add (randomizer(8)|>int)
            
            tempList <- ResizeArray.distinct(tempList) 
            printfn "%i" (ResizeArray.length(tempList))
        printfn "end"
        tempList
            //printfn "%s" nodeidList.[i] 
(*        let mutable tempList = ResizeArray.distinct(nodeidList) 
        let mutable length = ResizeArray.length(tempList) 
        let mutable deviation = numNodes -1 - length*)

            

        
(*    let uniqueNodeid(array:ResizeArray<_>,numNodes:int) = 
        let mutable 
        let tempList = generateId(numNodes)
        let mutable deviation = numNodes - 1 - tempList.FindLastIndex
        while deviation == 0 do *)
            
        
        
            
    // until now, the generteUniqueId would return an Array<string> which contains a lot of nodeids which may have repetition.
    //the following would solve the repetition.
    
    //let tempList = generateUniqueId(numNodes)//  numNodes from command line
    //let finalListId = tempList |> Seq.distinct |> List.ofSeq  //finalListId 就是最后需要的那一串nodeid，每一个都存在list中
    //这里，finallistid有可能真的去掉一些重复的，导致实际的节点数量小于numNodes，所以需要再次进行操作，直到finalListId.length=Numnodes
    //let doRandom(length,lists) = 
        


        

    


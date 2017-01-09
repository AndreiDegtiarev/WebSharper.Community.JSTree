namespace WebSharper.Community.JSTree.Sample

open WebSharper
open WebSharper.JavaScript
open WebSharper.Html.Client
open WebSharper.Community.JSTree

[<JavaScript>]
module Client =

    let Start input k =
        async {
            let! data = Server.DoSomething input
            return k data
        }
        |> Async.Start

    let Main () =
        let input = Input [Attr.Value ""] -< []
        let output = H1 []

        let treeDiv = Div[]
        let root=TreeNode(Text="Root",
                          State=TreeNodeState(Opened=true),
                          Children=[|
                                        TreeNode(Text="Child 1",Id="Child_1")
                                        TreeNode(Text="Child 2",Id="Child_2")
                                    |] 
                          )
        let core=TreeCore(Data=root)
        let settings=TreeSettings(Expand_selected_onload=true,Core=core)
        //Tree.Destroy(div.Dom)
        let jsTreeJson=Tree.Create(treeDiv.Dom,settings)
        let callbackFnc (val1:obj,val2:ChangedEventData) = output.Text <- ("Selected Id: "+val2.Selected.ToString())
        jsTreeJson.OnChange(callbackFnc)
                                                
        Div [
            input
            Button [Text "Send"]
            |>! OnClick (fun _ _ ->
                async {
                    let! data = Server.DoSomething input.Value
                    output.Text <- data
                }
                |> Async.Start
            )
            HR []
            H4 [Attr.Class "text-muted"] -< [Text "The server responded:"]
            Div [Attr.Class "jumbotron"] -< [output]
            treeDiv
        ]

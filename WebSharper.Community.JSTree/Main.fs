namespace Extension3

open WebSharper
open WebSharper.JavaScript
open WebSharper.InterfaceGenerator

module Definition =

    let TreeNodeState =
        Pattern.Config "TreeNodeState" {
            Required = []
            Optional =
                [
                    "opened" , T<bool>
                    "selected" , T<bool>
                ]
        } 
    let TreeNode =  Class "TreeNode"
    TreeNode  |+> Pattern.RequiredFields [
                "id", T<string>
                "text", T<string>
            ] |>ignore
    TreeNode |+> Pattern.OptionalFields [
                "state", TreeNodeState.Type
                "children",  !| TreeNode.Type
            ] |>ignore


    let TreeCore =
        Pattern.Config "TreeCore" {
            Required = []
            Optional =
                [
                    "data" , T<obj>
                ]
        }    
    let TreeSettings =
        Pattern.Config "TreeSettings" {
            Required = [  ]
            Optional =
                [
                    "core" , TreeCore.Type
                    "expand_selected_onload" , T<bool>
                    "animation",T<int>
                ]
        }
    let ChangedEventType =
        Pattern.EnumStrings "ChangedEventType" [ "selected_node"]
    let ChangedEventData =
        Pattern.Config "ChangedEventData" {
            Required = []
            Optional =
                [
                    "selected" , T<obj>
                ]
        } 
    let Tree = Class "Tree"
    Tree|+> Static [
         "create" => T<Dom.Element>?el * !?TreeSettings?settings ^-> Tree.Type |>  WithInline  "jQuery($el).jstree($settings)"
         "destroy" => T<Dom.Element>?el ^-> T<unit> |>  WithInline  "jQuery($el).jstree().destroy()" 

               ]|>ignore
    Tree|+> Instance [
          "onChange" => (T<obj> * ChangedEventData.Type ^-> T<unit>)?callback ^-> T<unit>|> WithInteropInline (fun tr -> "$this.on('changed.jstree', " + tr "callback" + ")")
                 ]|>ignore
    let Assembly =
        Assembly [
            Namespace "WebSharper.Community.JSTree" [
                TreeNodeState
                TreeNode
                TreeCore
                TreeSettings
                ChangedEventType
                ChangedEventData
                Tree
            ]
            Namespace "WebSharper.Community.JSTree.Resources" [
               // Resource "Extension3" "https://cdnjs.cloudflare.com/ajax/libs/jstree/3.3.3/jstree.min.js"
                Resource "Extension3" "/Scripts/jstree.js"
                |> fun r -> r.AssemblyWide()
            ]
        ]

[<Sealed>]
type Extension() =
    interface IExtension with
        member ext.Assembly =
            Definition.Assembly

[<assembly: Extension(typeof<Extension>)>]
do ()

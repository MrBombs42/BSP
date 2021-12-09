using System;
using System.Collections.Generic;
using BSP.Assets.Code.BSP;
using BSP.Assets.Code.BSP.SpaceParticion;
using UnityEngine;

namespace BSP.Assets.Code.Dungeon
{    
    public class DungeonCreator : MonoBehaviour {
        [Range (1, 400), SerializeField] private int _widthRange;
        [Range (0, 10), SerializeField] private int _splitQuantity = 4;
        [SerializeField] private bool _discartDungeonByMinRatio = true;
        [SerializeField] private float _minWidthContainerRation = 0.15f;
        [SerializeField] private float _minHeightContainerRation = 0.15f;

        [SerializeField] private float _minWidthRoomRation = 0.15f;
        [SerializeField] private float _minHeightRoomRation = 0.15f;

        private ITree<SpaceParticionData> _bspTree;

        private void Awake()
        {
            _bspTree = new SpaceParticionTree();
        }

        public void GenerateDungeon(){
            var rootData = new SpaceParticionData(){
                Container = new RectInt(0,0, _widthRange, _widthRange)
            };
            var root = _bspTree.CreateNode(null, rootData, "Root");
            _bspTree.SetRoot(root);
            root = Split(_splitQuantity, root);
            List<INode<SpaceParticionData>> leafs = new List<INode<SpaceParticionData>>();
            _bspTree.GetLeafs(ref leafs, root);
            foreach (var leaf in leafs)
            {
                leaf.Data.Room = GenerateRoom(leaf.Data.Container, leaf.Parent.Data.SplitDirection);
            }
        }

        public void PrintLog(){
            _bspTree.Print();
        }


        private INode<SpaceParticionData> Split(int numOfOperations, INode<SpaceParticionData> root)
        {          
            if(numOfOperations == 0){
                return root;
            }

            var splitedNode = SplitNode(root, numOfOperations);

            root.Left = Split(numOfOperations - 1, splitedNode.Item1);
            root.Right = Split(numOfOperations - 1, splitedNode.Item2);            

            return root;
        }

        private (INode<SpaceParticionData>, INode<SpaceParticionData>) SplitNode(INode<SpaceParticionData> node, int treeLevel)
        {
            var splitDirection = UnityEngine.Random.Range(0f, 1f) > 0.5f ? SplitDirection.Horizontal : SplitDirection.Vertical;
            var nodeContainer = node.Data.Container;

            INode<SpaceParticionData> leftNode;
            INode<SpaceParticionData> rightNode;
            SpaceParticionData leftData;
            SpaceParticionData rightData;           
            RectInt leftContainer;
            RectInt leftRoom = new RectInt();
            RectInt rightContainer;
            RectInt rightRoom = new RectInt();

            node.Data.SplitDirection = splitDirection;

            if(splitDirection == SplitDirection.Vertical){
                leftContainer = new RectInt(nodeContainer.x, nodeContainer.y, 
                    (int)(nodeContainer.width * UnityEngine.Random.Range(0.3f, 0.6f)), 
                    nodeContainer.height);         
               
                rightContainer = new RectInt(nodeContainer.x + leftContainer.width, 
                    nodeContainer.y, nodeContainer.width - leftContainer.width, 
                    nodeContainer.height);

                if(_discartDungeonByMinRatio){
                    var leftRatio = leftContainer.width / leftContainer.height;
                    var rightRatio = rightContainer.width / rightContainer.height;
                    if(leftRatio < _minWidthContainerRation || rightRatio < _minWidthContainerRation){
                        UnityEngine.Debug.LogError($"Discart ratio {leftRatio}, {rightRatio}");
                        return SplitNode(node, treeLevel);
                    }
                }
                               
                leftData = CreateSpaceParticionData(leftContainer, leftRoom);
                rightData = CreateSpaceParticionData(rightContainer, rightRoom);
            }
            else
            {
                leftContainer = new RectInt(nodeContainer.x, nodeContainer.y, 
                    nodeContainer.width, 
                    (int)(nodeContainer.height * UnityEngine.Random.Range(0.3f, 0.6f)));
                           
                rightContainer = new RectInt(nodeContainer.x, 
                    nodeContainer.y + leftContainer.height, nodeContainer.width, 
                    nodeContainer.height - leftContainer.height);

                if(_discartDungeonByMinRatio){
                    var leftRatio = leftContainer.width / leftContainer.height;
                    var rightRatio = rightContainer.width / rightContainer.height;
                    if(leftRatio < _minHeightContainerRation || rightRatio < _minHeightContainerRation){
                        UnityEngine.Debug.LogError($"Discart ratio {leftRatio}, {rightRatio}");
                        return SplitNode(node, treeLevel);
                    }
                }               

                leftData = CreateSpaceParticionData(leftContainer, leftRoom);
                rightData = CreateSpaceParticionData(rightContainer, rightRoom);                
            }

            leftNode = _bspTree.CreateNode(node, leftData, $"_left {treeLevel}");
            rightNode = _bspTree.CreateNode(node, rightData, $"_right {treeLevel}");

            return (leftNode, rightNode);
        }

        private SpaceParticionData CreateSpaceParticionData(RectInt container, RectInt room){
            return new SpaceParticionData{                     
                    Container = container,
                    Room = room
                };
        }

        private RectInt GenerateRoom(RectInt container, SplitDirection splitDirection){

            var x = container.x + Mathf.FloorToInt(UnityEngine.Random.Range(0, container.width/3));
            var y = container.y + Mathf.FloorToInt(UnityEngine.Random.Range(0, container.height/3));
            var width = container.width - (x - container.x);
            var height = container.height - (y - container.y);
            width -= UnityEngine.Random.Range(0, width/3);
            height -= UnityEngine.Random.Range(0, height/3);
            var ratio = width / height;
            // if(splitDirection == SplitDirection.Horizontal){
            //     if(ratio < _minHeightRoomRation){
            //         return GenerateRoom(container, splitDirection);
            //     }
            // }else{
            //     if(ratio < _minWidthRoomRation){
            //         return GenerateRoom(container, splitDirection);
            //     }
            // }

            return new RectInt(x, y, width, height);                 
        }

        private void DebugDrawBspTree(INode<SpaceParticionData> node){
            var nodeContainter = node.Data.Container;
            var nodeRoom = node.Data.Room;
            Gizmos.color = Color.green;

            DrawRectInt(nodeContainter);
            Gizmos.color = Color.red;        
            Gizmos.DrawCube(new Vector3(nodeRoom.center.x, nodeRoom.center.y, 0), new Vector3(nodeRoom.size.x, nodeRoom.size.y, 0));    
            //DrawRectInt(nodeRoom);

            if(node.Left != null){
                DebugDrawBspTree(node.Left);
            }
            if(node.Right != null){
                DebugDrawBspTree(node.Right);
            }
        }

        private void DrawRectInt(RectInt rectInt){
             //top
            Gizmos.DrawLine(new Vector3(rectInt.x, rectInt.y, 0), new Vector3(rectInt.xMax, rectInt.y, 0));
            //right
            Gizmos.DrawLine(new Vector3(rectInt.xMax, rectInt.y, 0), new Vector3(rectInt.xMax, rectInt.yMax, 0));
            //botton
            Gizmos.DrawLine(new Vector3(rectInt.x, rectInt.yMax, 0), new Vector3(rectInt.xMax, rectInt.yMax, 0));
            //left
            Gizmos.DrawLine(new Vector3(rectInt.x, rectInt.y, 0), new Vector3(rectInt.x, rectInt.yMax, 0));
        }

        private void OnDrawGizmos(){
            if(_bspTree != null && _bspTree.Root != null){

                DebugDrawBspTree(_bspTree.Root);
            }
            
        }
        
        
    }
}

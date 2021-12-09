using System;
using BSP.Assets.Code.BSP;
using BSP.Assets.Code.BSP.SpaceParticion;
using UnityEngine;

namespace BSP.Assets.Code.Dungeon
{    
    public class DungeonCreator : MonoBehaviour {
        [Range (1, 400), SerializeField] private int _widthRange;
        [Range (0, 10), SerializeField] private int _splitQuantity = 4;

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
        }

        public void PrintLog(){
            _bspTree.Print();
        }


        private INode<SpaceParticionData> Split(int numOfOperations, INode<SpaceParticionData> root)
        {          
            if(numOfOperations == 0){
                return root;
            }

            var splitedNode = SplitNode(root);

            Debug.Log(numOfOperations);
            root.Left = Split(numOfOperations - 1, splitedNode.Item1);

            Debug.Log(numOfOperations);
            root.Right = Split(numOfOperations - 1, splitedNode.Item2);
            

            return root;
        }

        private (INode<SpaceParticionData>, INode<SpaceParticionData>) SplitNode(INode<SpaceParticionData> node)
        {
            var splitDirection = UnityEngine.Random.Range(0f, 1f) > 0.5f ? SplitDirection.Horizontal : SplitDirection.Vertical;
            var nodeContainer = node.Data.Container;

            INode<SpaceParticionData> leftNode;
            INode<SpaceParticionData> rightNode;
            SpaceParticionData leftData;
            SpaceParticionData rightData;
            node.Data.SplitDirection = splitDirection;
            RectInt leftContainer;
            RectInt leftRoom;
            RectInt rightContainer;
            RectInt rightRoom;
            if(splitDirection == SplitDirection.Vertical){
                leftContainer = new RectInt(nodeContainer.x, nodeContainer.y, 
                    (int)(nodeContainer.width * UnityEngine.Random.Range(0.3f, 0.6f)), 
                    nodeContainer.height);
                leftRoom = GenerateRoom(leftContainer);
                leftData = CreateSpaceParticionData(leftContainer, leftRoom);

                rightContainer = new RectInt(nodeContainer.x + leftData.Container.width, 
                    nodeContainer.y, nodeContainer.width - leftData.Container.width, 
                    nodeContainer.height);
                rightRoom = GenerateRoom(rightContainer);
 
                rightData = CreateSpaceParticionData(rightContainer, rightRoom);
            }
            else
            {

                leftContainer = new RectInt(nodeContainer.x, nodeContainer.y, 
                    nodeContainer.width, 
                    (int)(nodeContainer.height * UnityEngine.Random.Range(0.3f, 0.6f)));
                leftRoom = GenerateRoom(leftContainer);
                leftData = CreateSpaceParticionData(leftContainer, leftRoom);

                rightContainer = new RectInt(nodeContainer.x, 
                    nodeContainer.y + leftData.Container.height, nodeContainer.width, 
                    nodeContainer.height - leftData.Container.height);
                rightRoom = GenerateRoom(rightContainer);
 
                rightData = CreateSpaceParticionData(rightContainer, rightRoom);                
            }

            leftNode = _bspTree.CreateNode(node, leftData, node.Id.Insert(node.Id.Length, "_left"));
            rightNode = _bspTree.CreateNode(node, rightData, node.Id.Insert(node.Id.Length, "_right"));

            return (leftNode, rightNode);
        }

        private SpaceParticionData CreateSpaceParticionData(RectInt container, RectInt room){
            return new SpaceParticionData{                     
                    Container = container,
                    Room = room
                };
        }

        private RectInt GenerateRoom(RectInt container){
            var x = container.x + UnityEngine.Random.Range(0, container.xMax/2);
            var y = container.y + UnityEngine.Random.Range(0, container.yMax/2);
            var width = UnityEngine.Random.Range((container.xMax-x)/4, container.xMax-x);
            var height = UnityEngine.Random.Range((container.yMax-y)/4, container.yMax-y);

            return new RectInt(x, y, width, height);                 
        }

        private void DebugDrawBspTree(INode<SpaceParticionData> node){
            var nodeContainter = node.Data.Container;
            var nodeRoom = node.Data.Room;
            Gizmos.color = Color.green;

            DrawRectInt(nodeContainter);
            Gizmos.color = Color.red;    
            DrawRectInt(nodeRoom);

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

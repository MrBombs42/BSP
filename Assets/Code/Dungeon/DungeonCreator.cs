using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private int _hallSize = 1;
        [SerializeField] private float _debugStepTimeInSeconds = 1;

        private const float MinRandomSplitValue = 0.45f;
        private const float MaxRandomSplitValue = 0.55f;

        private ITree<SpaceParticionData> _bspTree;
        private List<RectInt> _halls;

        private void Awake()
        {
            _bspTree = new SpaceParticionTree();
        }

        public void GenerateDungeon()
        {
            var rootData = new SpaceParticionData()
            {
                Container = new RectInt(0, 0, _widthRange, _widthRange)
            };

            _halls = new List<RectInt>();
            var root = _bspTree.CreateNode(null, rootData, "Root");
            _bspTree.SetRoot(root);
            root = Split(_splitQuantity, root);
            List<INode<SpaceParticionData>> leafs = new List<INode<SpaceParticionData>>();
            _bspTree.GetLeafs(ref leafs, root);
            foreach (var leaf in leafs)
            {
                leaf.Data.Room = GenerateRoom(leaf.Data.Container, leaf.Parent.Data.SplitDirection);
            }

            HashSet<INode<SpaceParticionData>> leafParents = new HashSet<INode<SpaceParticionData>>();
            _bspTree.GetLeafsParent(ref leafParents, root);

            StartCoroutine(WaitAndGenerateHall(leafParents));

        }

        private IEnumerator WaitAndGenerateHall(HashSet<INode<SpaceParticionData>> leafParents)
        {
            foreach (var leafParent in leafParents)
            {
               CreateHall(ref _halls, leafParent.Left.Data.Room, leafParent.Right.Data.Room, leafParent.Data.SplitDirection);
                yield return new WaitForSeconds(_debugStepTimeInSeconds);
            }

            var leafParentList = leafParents.ToList();
            //UnityEngine.Debug.LogError("last hall");
            //for (int i = 0; i < leafParentList.Count - 1; i += 2)
            //{
            //    CreateHall(ref _halls, leafParentList[i].Data.Container, leafParentList[i + 1].Data.Container);
            //    yield return new WaitForSeconds(1);
            //}
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
                    (int)(nodeContainer.width * UnityEngine.Random.Range(MinRandomSplitValue, MaxRandomSplitValue)),
                    nodeContainer.height);

                rightContainer = new RectInt(nodeContainer.x + leftContainer.width,
                    nodeContainer.y, nodeContainer.width - leftContainer.width,
                    nodeContainer.height);

                if(_discartDungeonByMinRatio){
                    var leftRatio = leftContainer.width / leftContainer.height;
                    var rightRatio = rightContainer.width / rightContainer.height;
                    if(leftRatio < _minWidthContainerRation || rightRatio < _minWidthContainerRation
                    || leftRatio > 5 || rightRatio> 5){
                       // UnityEngine.Debug.LogError($"Discart ratio {leftRatio}, {rightRatio}");
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
                    (int)(nodeContainer.height * UnityEngine.Random.Range(MinRandomSplitValue, MaxRandomSplitValue)));

                rightContainer = new RectInt(nodeContainer.x,
                    nodeContainer.y + leftContainer.height, nodeContainer.width,
                    nodeContainer.height - leftContainer.height);

                if(_discartDungeonByMinRatio){
                    var leftRatio = leftContainer.width / leftContainer.height;
                    var rightRatio = rightContainer.width / rightContainer.height;
                    if(leftRatio < _minHeightContainerRation || rightRatio < _minHeightContainerRation
                     || leftRatio > 5 || rightRatio> 5){
                        //UnityEngine.Debug.LogError($"Discart ratio {leftRatio}, {rightRatio}");
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
            //restrain split
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

        public void CreateHall(ref List<RectInt> halls, RectInt leftRegion, RectInt rightRegion, SplitDirection splitDirection)
        {
            //desenhar q fica mais facil
            var halfHallSize = _hallSize / 2;
            if (splitDirection == SplitDirection.Horizontal)
            {
                Debug.Log("Horizontal");
                var rnd = UnityEngine.Random.Range(leftRegion.xMin + halfHallSize, leftRegion.xMax - halfHallSize);
                if(rnd < rightRegion.xMin || rnd > rightRegion.xMax)
                {
                    var opositeRnd = UnityEngine.Random.Range(rightRegion.yMin, rightRegion.yMax);
                    if (rnd < rightRegion.xMin)
                    {
                        Debug.Log("XMin");

                        halls.Add(new RectInt(rnd, leftRegion.yMax, _hallSize, Math.Abs(opositeRnd - leftRegion.yMax)));
                        halls.Add(new RectInt(rightRegion.xMin, opositeRnd, -Math.Abs(rnd - rightRegion.xMin), _hallSize));
                    }
                    else
                    {
                        Debug.Log("XMax");
                        halls.Add(new RectInt(rnd, leftRegion.yMax, _hallSize, Math.Abs(opositeRnd - leftRegion.yMax)));
                        halls.Add(new RectInt(rightRegion.xMax, opositeRnd, Math.Abs(rnd - rightRegion.xMax) + _hallSize, _hallSize));
                    }
                }
                else
                {
                    Debug.Log("Reto");
                    var h = Math.Abs(rightRegion.yMin - leftRegion.yMax);
                    halls.Add(new RectInt(rnd, leftRegion.yMax, _hallSize, h));
                }
            }
            else
            {
                Debug.Log("Vertical");
                var rnd = UnityEngine.Random.Range(rightRegion.yMin + _hallSize, rightRegion.yMax - _hallSize);
                if (rnd < rightRegion.yMin || rnd > rightRegion.yMax)
                {
                    var opositeRnd = UnityEngine.Random.Range(rightRegion.xMin, rightRegion.xMax);
                    if (rnd < rightRegion.yMin)
                    {
                        Debug.Log("yMin");
                        halls.Add(new RectInt(leftRegion.xMax, rnd, Math.Abs(opositeRnd - leftRegion.xMax), _hallSize));
                        halls.Add(new RectInt(opositeRnd, rightRegion.yMin, _hallSize, Math.Abs(rnd - rightRegion.yMax)));
                    }
                    else
                    {
                        Debug.Log("yMax");
                        halls.Add(new RectInt(leftRegion.xMax, rnd, Math.Abs(opositeRnd - leftRegion.xMax), _hallSize));
                        halls.Add(new RectInt(opositeRnd, rightRegion.yMax, _hallSize, -Math.Abs(rnd - rightRegion.yMax)));
                    }
                }
                else
                {
                    Debug.Log("Reto");
                    var w = Math.Abs(rightRegion.xMin - leftRegion.xMax);
                    halls.Add(new RectInt(leftRegion.xMax, rnd, w, _hallSize));
                }
            }
        }

        private void DebugDrawBspTree(INode<SpaceParticionData> node){
            var nodeContainter = node.Data.Container;
            var nodeRoom = node.Data.Room;
            Gizmos.color = Color.green;

            DrawRectInt(nodeContainter);
            DrawRoom(nodeRoom);

            if(node.Left != null){
                DebugDrawBspTree(node.Left);
            }
            if(node.Right != null){
                DebugDrawBspTree(node.Right);
            }
        }

        private void DrawHall(RectInt hall){
            Gizmos.color = Color.black;
            Gizmos.DrawCube(new Vector3(hall.center.x, hall.center.y, 0), new Vector3(hall.size.x, hall.size.y, 0));
        }

        private void DrawRoom(RectInt nodeRoom){
            Gizmos.color = Color.red;
            Gizmos.DrawCube(new Vector3(nodeRoom.center.x, nodeRoom.center.y, 0), new Vector3(nodeRoom.size.x, nodeRoom.size.y, 0));
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

            if(_halls != null && _halls.Count > 0){
                foreach (var hall in _halls)
                {
                    DrawHall(hall);
                }
            }

        }


    }
}

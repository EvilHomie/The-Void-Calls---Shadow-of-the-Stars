//using Enviroment;
//using GamePool;
//using UnityEngine;

//namespace GameSystem
//{
//    public class StarsPool : AbstractPool<TwinkleStar>
//    {
//        [SerializeField] TwinkleStar _starPF;
//        [SerializeField] int _startCapacity;
//        [SerializeField] int _maxCapacity;
//        [SerializeField] int _prewarmCount;
//        private GameObject _projectileContainer;

//        private void Awake()
//        {
//            _projectileContainer = new GameObject(_starPF.ObjectName);
//            _projectileContainer.transform.SetParent(transform, false);
//            CreateItemPool(_starPF, _startCapacity, _maxCapacity, _projectileContainer.transform, _prewarmCount);
//        }

//        protected override void Subscribe()
//        {
//            EventBus.DestroyStar += ScheduleForRelease;
//            EventBus.GetStar += Getitem;
//            GameFlow.GameTickStandart += ReleasePendingItems;


//            //GameFlowSystem.ChangeGameState += OnChangeGameState;
//        }

//        protected override void Unsubscribe()
//        {
//            EventBus.DestroyStar -= ScheduleForRelease;
//            EventBus.GetStar -= Getitem;
//            GameFlow.GameTickStandart -= ReleasePendingItems;

//            //GameFlowSystem.ChangeGameState -= OnChangeGameState;
//        }

//        //private void OnChangeGameState(GameState gameState) //Пока нет реализвации состояний игры. Но думаю будут
//        //{
//        //    if (gameState == GameState.GameOver || gameState == GameState.Victory)
//        //    {
//        //        ReleaseAll();
//        //    }
//        //}
//    }
//}

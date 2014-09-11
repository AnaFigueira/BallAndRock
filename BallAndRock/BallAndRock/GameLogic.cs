using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BallAndRock
{
    public class GameLogic
    {
        Random rand;
        int _highScore = 0;
        int _score = 0;
        int _columns = 0;
        int _rows = 0;
        bool _playerLost = false;

        public GameLogic(int nColumns, int nRows)
        {
            rand = new Random((int)DateTime.Now.Ticks);
            _columns = nColumns;
            _rows = nRows;
            _score = 0;
        }
        
        public void Start()
        {
            _score = 0;
            while(!_playerLost)
            {
                

                if (_playerLost)
                    SaveScore();
            }
        }

        private void SaveScore()
        {
            if (_score >= _highScore)
                _highScore = _score;

            // TODO: Save score
        }

        
        public int Stop()
        {
            return _score;

        }

        int GetRandomRockPosition()
        {
            //RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            //byte[] buffer = new byte[sizeof(Int32)];

            //rng.GetBytes(buffer);
            //int result = BitConverter.ToInt32(buffer, 0);
            
            return rand.Next(0, _columns);
        }


    }
}

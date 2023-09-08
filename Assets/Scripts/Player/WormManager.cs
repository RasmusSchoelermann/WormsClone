using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormManager : MonoBehaviour
{
    [SerializeField]
    private PlayerNetworkObject _player;

    private List<Worm> _playerWorms = new List<Worm>();

    //private bool _playerTurn = false;

    private int _currentWormId = 0;

    public void SetUpWormManager(List<Worm> worms)
    {
        if (worms.Count <= 0)
            return;
        //SETUP FOR LATER SPAWNING OF WORMS
        _playerWorms = worms;
    }

    public void SwitchWorms()
    {
        _playerWorms = _player.ReturnClientWormList();
        if (CurrentWormValidation() == true)
        {
            _player.currentWorm = _playerWorms[0];
            SelectWorm(_playerWorms[0]);
            return;
        }

        _currentWormId++;
        if (_currentWormId >= _playerWorms.Count)
            _currentWormId = 0;

        SelectWorm(_playerWorms[_currentWormId]);

    }

    private void SelectWorm(Worm worm)
    {
        //Transform wormTransform = worm.gameObject.transform;
        _player.currentWorm = worm;

        //TODO Main Camera Panning to Current Worm
    }

    private bool CurrentWormValidation()
    {
        return _player.currentWorm == null && _playerWorms.Count > 0;
    }
}

using Godot;
using System;

public class Player : KinematicBody2D
{
    private bool goToMid = false;
    private bool goToStartPos = false;
    private bool targetChoose = false;
    private int targetIndex = 0;

    public void SetTargetChoose(bool a) {targetChoose = a;}
    public void SetGoToMid(bool a) {goToMid = a;}
    public void SetGoToStartPos(bool a) {goToStartPos = a;}
    private Vector2 target = new Vector2(680, 700);
    private Vector2 startTarget;
    public void SetStartTarget(Vector2 a) {startTarget = a;}
    private GUI gui;
    private Stats stats;
    private BattleManager battleManager;
    private float speed = 39000;
    private float timer = 0;
    public void SetTimer(float a){timer = a;}
    private int attackDir = 0;
    public int GetAttackDirection() {return attackDir;}
    private bool chooseAttackDir = false;

    public override void _Ready()
    {
        gui = GetParent().GetParent().GetNode("UI") as GUI;
        battleManager = GetParent() as BattleManager;
        stats = GetNode("Stats") as Stats;
    }
 // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (goToMid)
        {
            GoToMiddle(delta);
        }

        if (goToStartPos)
        {
            GoToStartPos(delta);
        }

        if (targetChoose)
        {
            ChooseTarget(delta);
        }   

        if (chooseAttackDir)
        {
            ChooseAttackDirection();
        }  
    }

    private void GoToMiddle(float delta)
    {
        Vector2 vector = (target - Position).Normalized();
        MoveAndSlide(vector * speed * delta);

        if (Position.x > target.x)
        {
            GD.Print("There");
            goToMid = false;
            gui.ShowAttackMenu(stats);
        }
    }

    private void GoToStartPos(float delta)
    {
        Vector2 vector = (startTarget - Position).Normalized();
        MoveAndSlide(vector * speed * delta);

        if (Position.x < startTarget.x)
        {
            GD.Print("At startPos");
            goToStartPos = false;
            battleManager.CurrCharacterGoesToMid();
        }
    }

    private void ChooseTarget(float delta)
    {
        if (timer < 0.3f)
        {
            timer += delta;
        }        

        if (Input.IsActionJustPressed("ui_accept") && timer > 0.25f)
        {
            GD.Print(stats.GetCharName());
            battleManager.GetEnemies()[targetIndex].GetNode<CharacterDamage>("Damage").StartGuardSequence(stats);
            targetChoose = false;
            chooseAttackDir = true;
        }

        if (battleManager.GetEnemies().Count == 0) {return;}

        if (Input.IsActionJustPressed("ui_up"))
        {
            battleManager.GetEnemies()[targetIndex].GetNode<Sprite>("Marker").Visible = false;

            targetIndex++;

            if (targetIndex > battleManager.GetEnemies().Count - 1)
            {
                targetIndex = 0;
            }

            battleManager.GetEnemies()[targetIndex].GetNode<Sprite>("Marker").Visible = true;            
        }
        else if (Input.IsActionJustPressed("ui_down"))
        {
            battleManager.GetEnemies()[targetIndex].GetNode<Sprite>("Marker").Visible = false;

            targetIndex--;

            if (targetIndex < 0)
            {
                targetIndex = battleManager.GetEnemies().Count - 1;
            }
    
            battleManager.GetEnemies()[targetIndex].GetNode<Sprite>("Marker").Visible = true;            
        }
    }

    private void ChooseAttackDirection()
    {
        if (Input.IsActionPressed("ui_up"))
        {
            attackDir = 1;
        }
        else if (Input.IsActionPressed("ui_down"))
        {
            attackDir = -1;
        }
        else
        {
            attackDir = 0;
        } 
    }
}

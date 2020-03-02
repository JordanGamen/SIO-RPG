using Godot;
using System;
using System.Collections.Generic;

public enum AIskillTypes {HIGHESTATK, SUPPORT, EVERYONE, ATTACK};

public class EnemyAI : KinematicBody2D
{
    private Node specials;
    private List<AIskillTypes> learnList = new List<AIskillTypes>();
    public List<AIskillTypes> GetLearnList() {return learnList;}
    private BattleManager battleManager;
    private Stats stats;
    private CharacterDamage damageScript;
    private Timer timer;
    private bool timerStarted = false;
    private Skill chosenSkill;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        specials = GetNode("Special Moves");
        battleManager = GetParent() as BattleManager;
        stats = GetNode("Stats") as Stats;
        damageScript = GetNode("Damage") as CharacterDamage;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (timer != null && timerStarted)
        {
            if (timer.TimeLeft == 0)
            {                
                timerStarted = false;
                timer.Stop();
                timer.QueueFree();
                damageScript.EnemyGuardChoose();
            }            
        }
    }

    public void MyTurn()
    {
        while (!RandomSkill(specials))
        {
            GD.Print("Searching skill");
        }

        int count = battleManager.GetPlayers().Count;

        Random rand = new Random();
        int num = rand.Next() % count;

        if (chosenSkill != null)
        {
            GD.Print("Skill: " + chosenSkill.GetAtk());
            stats.SetStamina(stats.GetStamina() - chosenSkill.GetStaminaDepletion());
            damageScript.GetStaminaBar().Value = (float)stats.GetStamina() / (float)stats.GetMaxStamina() * 100;

            if (chosenSkill.GetAttackAll())
            {
                for (int i = 0; i < battleManager.GetPlayers().Count; i++)
                {
                    battleManager.GetPlayers()[i].GetNode<CharacterDamage>("Damage").StartGuardSequence(stats, chosenSkill, null);
                }                
            }
            else
            {
                battleManager.GetPlayers()[num].GetNode<CharacterDamage>("Damage").StartGuardSequence(stats, chosenSkill, null);
            }
        }
        else
        {
            battleManager.GetPlayers()[num].GetNode<CharacterDamage>("Damage").StartGuardSequence(stats, chosenSkill, null);
        }        

        damageScript.EnemyGuardChoose();
        timer = new Timer();
        timer.WaitTime = 0.4f;
        timer.OneShot = true;
        AddChild(timer);
        timer.Start();
        timerStarted = true;
    }

    private bool RandomSkill(Node specials)
    {
        bool skillFound = true;

        Random rand = new Random();

        int numSkill = rand.Next() % 5;
        if (numSkill > 0)
        {
            chosenSkill = specials.GetChild(numSkill - 1) as Skill;
            if (chosenSkill.GetStaminaDepletion() >= stats.GetStamina())
            {
                skillFound = false;
            }
        }
        else
        {
            chosenSkill = null;
        }

        return skillFound;
    }
}

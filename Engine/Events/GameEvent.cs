using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
namespace CORA
{
    public class GameEvent
    {
        public LevelState level;
        public GameState gameState;
        public List<String[]> commands;
        public int selectedIndex;
        public String[] command;
        public Boolean needsNextCommand;
        protected Hashtable objects;
        protected Hashtable textures;
        public CSLCommandType eventState;
        public List<HandledEvent> events;
        public List<HandledEvent> cleanup;
        public Boolean hasExecuted = false;
        public Boolean instructionHasCompleted = false;
        public Boolean waitingForCommand = false;
        public GameEvent(GameState gameState, LevelState level)
        {
            this.gameState = gameState;
            this.level = level;
            commands = new List<String[]>();
            needsNextCommand = false;
            objects = new Hashtable();
            textures = new Hashtable();
            selectedIndex = 0;
            events = new List<HandledEvent>();
            cleanup = new List<HandledEvent>();
        }
        public void loadScript(string path, string name, ContentManager c)
        {
            try
            {
                FileStream stream = new FileStream(path, FileMode.Open);
                StreamReader reader = new StreamReader(stream);
                string s = "";
                if(!reader.EndOfStream)
                    s = reader.ReadLine();
                while (s != "begin script " + name)
                    s = reader.ReadLine();
                while (!reader.EndOfStream && reader.Peek() != '@')
                {
                    command = reader.ReadLine().Split(' ');
                    CSLCommandType o = (CSLCommandType)Enum.Parse(typeof(CSLCommandType), command[0]);
                    switch (o)
                    {
                        case CSLCommandType.spawn:
                            Texture2D tex = c.Load<Texture2D>(command[2]);
                            if(!textures.ContainsKey(command[2]))
                                textures.Add(command[2], tex);
                            break;
                        case CSLCommandType.displaytext:
                            SpriteFont font = c.Load<SpriteFont>(command[2]);
                            if(!textures.ContainsKey(command[2]))
                                textures.Add(command[2], font);
                            break;
                        default:
                            break;
                    }
                    commands.Add(command);
                }
                if(reader.Peek() == '@')
                    commands.Add(new string[1]{"end"});
            }
            catch (Exception ex)
            {}
        }
        public void doThis(doPacket pack)
        {
            if (waitingForCommand && instructionHasCompleted)
            {
                waitingForCommand = false;
                instructionHasCompleted = false;
                needsNextCommand = true;
            }
            if (needsNextCommand)
                parseCommand();
            foreach (HandledEvent e in events)
                e.doThis(pack);

        }
        public void parseCommand()
        {
            command = commands[selectedIndex];
            selectedIndex++;
            CSLCommandType o = (CSLCommandType)Enum.Parse(typeof(CSLCommandType), command[0]);
            if (command[0] == "@")
                o = CSLCommandType.end;
            switch (o)
            {
                case CSLCommandType.create:
                    parseCreate(false);
                    break;
                case CSLCommandType.delete:
                    parseDelete(false);
                    break;
                case CSLCommandType.displaytext:
                    parseDisplayText();
                    break;
                case CSLCommandType.drawableEnabled:
                    parseDrawableEnabled();
                    break;
                case CSLCommandType.drawableVisible:
                    parseDrawableVisibile();
                    break;
                case CSLCommandType.end:
                    end();
                    break;
                case CSLCommandType.fade:
                    parseFade();
                    break;
                case CSLCommandType.givePlayerControl:
                    parseGivePlayerControl();
                    break;
                case CSLCommandType.go:
                    parseGo();
                    break;
                case CSLCommandType.move:
                    parseMove();
                    break;
                case CSLCommandType.moveCamera:
                    parseMoveCamera();
                    break;
                case CSLCommandType.despawn:
                    parseDelete(true);
                    break;
                case CSLCommandType.slideCamera:
                    parseSlideCamera();
                    break;
                case CSLCommandType.spawn:
                    parseCreate(true);
                    break;
                case CSLCommandType.takePlayerControl:
                    parseTakePlayerControl();
                    break;
                case CSLCommandType.waitforinstruction:
                    parseWaitForInstruction();
                    break;
                case CSLCommandType.waittime:
                    parseWaitTime();
                    break;
                case CSLCommandType.walk:
                    parseWalk();
                    break;
                default:
                    break;
            }            
        }
        public void parseCreate(Boolean spawn)
        {
            if (command[1].StartsWith("$") && spawn)
            {
                if (objects.ContainsKey(command[1]))
                {
                    CSLObjectType o = ((ScriptedObject)objects[command[1]]).type;
                    switch (o)
                    {
                        case CSLObjectType.animateddoodad:
                            level.doodads.Add((AnimatedDoodad)(((ScriptedObject)objects[command[1]]).o));
                            break;
                        case CSLObjectType.doodad:
                            level.doodads.Add((Doodad)(((ScriptedObject)objects[command[1]]).o));
                            break;
                        case CSLObjectType.elevatorsurface:
                            level.interactables.Add((ElevatorSurface)(((ScriptedObject)objects[command[1]]).o));
                            break;
                        case CSLObjectType.movingplatform:
                            level.walls.Add((MovingPlatform)(((ScriptedObject)objects[command[1]]).o));
                            break;
                        case CSLObjectType.player:
                            if (level.player != null)
                                level.objects.Remove(level.player);
                            gameState.player = (Player)(((ScriptedObject)objects[command[1]]).o);
                            level.player = (Player)(((ScriptedObject)objects[command[1]]).o);
                            level.objects.Add((Player)(((ScriptedObject)objects[command[1]]).o));
                            break;
                        case CSLObjectType.slope:
                            level.walls.Add((Slope)(((ScriptedObject)objects[command[1]]).o));
                            break;
                        case CSLObjectType.wall:
                            level.walls.Add((Wall)(((ScriptedObject)objects[command[1]]).o));
                            break;
                    }
                }
            }
            else
            {
                CSLObjectType o = (CSLObjectType)Enum.Parse(typeof(CSLObjectType), command[1]);
                object b;
                switch (o)
                {
                    case CSLObjectType.animateddoodad:
                        Boolean repeat = true;
                        if (command[7] == "false")
                            repeat = false;
                        b = new AnimatedDoodad((Texture2D)textures[command[2]], Int32.Parse(command[3]), Int32.Parse(command[4]), Int32.Parse(command[5]), Int32.Parse(command[6]), repeat, Int32.Parse(command[8]), parseCoordinateVector(command[9], command[10]));
                        if (command.Length > 11 && command[11].StartsWith("$"))
                        {
                            if (objects.ContainsKey(command[11]))
                                objects.Remove(command[11]);
                            objects.Add(command[11], new ScriptedObject(b, o));
                        }
                        if (spawn)
                            level.doodads.Add((AnimatedDoodad)b);
                        break;
                    case CSLObjectType.batterybot:
                        //FILL IN AFTER DEFINING
                        break;
                    case CSLObjectType.bucketbot:
                        //FILL IN AFTER DEFINING
                        break;
                    case CSLObjectType.controlpanel:
                        //NO SPAWNING YET
                        break;
                    case CSLObjectType.cutterbot:
                        //FILL IN AFTER DEFINING
                        break;
                    case CSLObjectType.doodad:
                        b = new Doodad((Texture2D)textures[command[2]], parseCoordinateVector(command[3], command[4]));
                        if (command.Length > 5 && command[5].StartsWith("$"))
                        {
                            if (objects.ContainsKey(command[5]))
                                objects.Remove(command[5]);
                            objects.Add(command[5], new ScriptedObject(b, o));
                        }
                        if (spawn)
                            level.doodads.Add((Doodad)b);
                        break;
                    case CSLObjectType.door:
                        //FILL IN AFTER DEFINING
                        break;
                    case CSLObjectType.elevatorbot:
                        //FILL IN AFTER DEFINING
                        break;
                    case CSLObjectType.elevatorsurface:
                        Boolean isRight = true;
                        if (command[6] == "false")
                            isRight = false;
                        b = new ElevatorSurface(parseHitBox(command[2], command[3], command[4], command[5]), level, (Texture2D)textures[command[1]], isRight, parseCoordinateVector(command[7], command[8]), parseCoordinateVector(command[9], command[10]));
                        if (command.Length > 11 && command[11].StartsWith("$"))
                        {
                            if (objects.ContainsKey(command[11]))
                                objects.Remove(command[11]);
                            objects.Add(command[11], new ScriptedObject(b, o));
                        }
                        if (spawn)
                            level.interactables.Add((ElevatorSurface)b);
                        break;
                    case CSLObjectType.hangingledge:
                        //FILL IN SOMETIME?
                        break;
                    case CSLObjectType.movinghangingledge:
                        //FILL IN SOMETIME?
                        break;
                    case CSLObjectType.movingplatform:
                        b = new MovingPlatform(parseHitBox(command[2], command[3], command[4], command[5]), level, parsePointFromVector(parseCoordinateVector(command[6], command[7])), parsePointFromVector(parseCoordinateVector(command[8], command[9])), MovingPlatformRotationType.Bouncing, float.Parse(command[10]), false, false, (Texture2D)textures[command[1]]);
                        if (command.Length > 11 && command[11].StartsWith("$"))
                        {
                            if (objects.ContainsKey(command[11]))
                                objects.Remove(command[11]);
                            objects.Add(command[11], new ScriptedObject(b, o));
                        }
                        if (spawn)
                            level.walls.Add((MovingPlatform)b);
                        break;
                    case CSLObjectType.player:
                        Player p = new Player((Texture2D)textures[command[1]], level.walls, level);
                        p.movePlayer(parsePointFromVector(parseCoordinateVector(command[2], command[3])));
                        if (level.player != null)
                        {
                            if (level.objects.Contains(level.player))
                                level.objects.Remove(level.player);
                        }
                        if (spawn)
                        {
                            gameState.player = p;
                            level.player = p;
                            level.objects.Add(p);
                        }
                        if (command.Length > 4 && command[4].StartsWith("$"))
                        {
                            if (objects.ContainsKey(command[4]))
                                objects.Remove(command[4]);
                            objects.Add(command[4], new ScriptedObject(p, o));
                        }
                        break;
                    case CSLObjectType.pressureplate:
                        //NO SPAWNING YET
                        break;
                    case CSLObjectType.rocketbot:
                        //FILL IN AFTER DEFINING
                        break;
                    case CSLObjectType.rust:
                        b = new Rust(parseHitBox(command[2], command[3], command[4], command[5]), level, double.Parse(command[6]), (Texture2D)textures[command[1]]);
                        if (command.Length > 7 && command[7].StartsWith("$"))
                        {
                            if (objects.ContainsKey(command[7]))
                                objects.Remove(command[7]);
                            objects.Add(command[7], new ScriptedObject(b, o));
                        }
                        if (spawn)
                            level.walls.Add((Rust)b);
                        break;
                    case CSLObjectType.slope:
                        b = new Slope(level, parsePointFromVector(parseCoordinateVector(command[2], command[3])), parsePointFromVector(parseCoordinateVector(command[4], command[5])), (Texture2D)textures[command[1]]);
                        if (command.Length > 6 && command[6].StartsWith("$"))
                        {
                            if (objects.ContainsKey(command[6]))
                                objects.Remove(command[6]);
                            objects.Add(command[6], new ScriptedObject(b, o));
                        }
                        if (spawn)
                            level.walls.Add((Slope)b);
                        break;
                    case CSLObjectType.swarmbot:
                        //FILL IN AFTER DEFINING
                        break;
                    case CSLObjectType.toolbot:
                        b = new Toolbot((Texture2D)textures[command[1]], level.walls, level, parseCoordinateVector(command[2], command[3]));
                        if (command.Length > 4 && command[4].StartsWith("$"))
                        {
                            if (objects.ContainsKey(command[4]))
                                objects.Remove(command[4]);
                            objects.Add(command[4], new ScriptedObject(b, o));
                        }
                        if (spawn)
                            level.objects.Add((Toolbot)b);
                        break;
                    case CSLObjectType.wall:
                        b = new Wall(parseHitBox(command[2], command[3], command[4], command[5]), level, (Texture2D)textures[command[1]]);
                        if (command.Length > 6 && command[6].StartsWith("$"))
                        {
                            if (objects.ContainsKey(command[6]))
                                objects.Remove(command[6]);
                            objects.Add(command[6], new ScriptedObject(b, o));
                        }
                        if (spawn)
                            level.walls.Add((Wall)b);
                        break;
                }
            }
            GC.Collect();
        }
        public void parseDelete(Boolean despawn)
        {
            if (command[1].StartsWith("$") && objects.ContainsKey(command[1]))
            {
                Object o = objects[command[1]];
                objects.Remove(command[1]);
                if (despawn)
                {
                    if (level.doodads.Contains(((ScriptedObject)o).o))
                        level.doodads.Remove((Doodad)((ScriptedObject)o).o);
                    else if (level.objects.Contains(((ScriptedObject)o).o))
                        level.objects.Remove((GameObject)o);
                    else if (level.interactables.Contains(((ScriptedObject)o).o))
                        level.interactables.Remove((HitBoxInteractable)((ScriptedObject)o).o);
                    else if (level.walls.Contains(o))
                        level.walls.Remove((LevelBlock)((ScriptedObject)o).o);
                }
            }
        }
        public void parseDisplayText()
        {
            events.Add(new DisplayTextEvent(gameState, level, this, (SpriteFont)((ScriptedObject)textures[command[1]]).o, command[2], double.Parse(command[3]), parseCoordinateVector(command[4], command[5]), command[6]));
        }
        public void parseDrawableEnabled()
        {
            ScriptedObject o = (ScriptedObject)objects[command[1]];
            Boolean b = true;
            if(command[2] == "false")
                b = false;
            switch (o.type)
            {
                case CSLObjectType.animateddoodad:
                case CSLObjectType.controlpanel:
                case CSLObjectType.doodad:
                case CSLObjectType.door:
                case CSLObjectType.elevatorsurface:
                case CSLObjectType.hangingledge:
                case CSLObjectType.movinghangingledge:
                case CSLObjectType.movingplatform:
                case CSLObjectType.pressureplate:
                case CSLObjectType.rust:
                case CSLObjectType.slope:
                case CSLObjectType.wall:
                    ((Drawable)(((ScriptedObject)objects[command[1]]).o)).enabled = b;
                    break;
                default:
                    ((GameObject)(((ScriptedObject)objects[command[1]]).o)).enabled = b;
                    break;
            }
        }
        public void parseDrawableVisibile()
        {
            ScriptedObject o = (ScriptedObject)objects[command[1]];
            Boolean b = true;
            if (command[2] == "false")
                b = false;
            switch (o.type)
            {
                case CSLObjectType.animateddoodad:
                case CSLObjectType.controlpanel:
                case CSLObjectType.doodad:
                case CSLObjectType.door:
                case CSLObjectType.elevatorsurface:
                case CSLObjectType.hangingledge:
                case CSLObjectType.movinghangingledge:
                case CSLObjectType.movingplatform:
                case CSLObjectType.pressureplate:
                case CSLObjectType.rust:
                case CSLObjectType.slope:
                case CSLObjectType.wall:
                    ((Drawable)(((ScriptedObject)objects[command[1]]).o)).visible = b;
                    break;
                default:
                    ((GameObject)(((ScriptedObject)objects[command[1]]).o)).visible = b;
                    break;
            }
        }
        public void parseFade()
        {
            events.Add(new FadeEvent(gameState, level, this, command[1], command[2], double.Parse(command[3])));
        }
        public void parseGivePlayerControl()
        {
            gameState.acceptPlayerInput = true;
        }
        public void parseGo()
        {
            selectedIndex = int.Parse(command[1]);
        }
        public void parseMove()
        {
            ScriptedObject o = (ScriptedObject)objects[command[1]];
            switch (o.type)
            {
                case CSLObjectType.doodad:
                case CSLObjectType.animateddoodad:
                    ((Doodad)o.o).PosX = parseCoordinate(command[2], true);
                    ((Doodad)o.o).PosY = parseCoordinate(command[3], false);
                    break;
                case CSLObjectType.controlpanel:
                case CSLObjectType.door:
                case CSLObjectType.elevatorsurface:
                case CSLObjectType.hangingledge:
                case CSLObjectType.movinghangingledge:
                case CSLObjectType.pressureplate:
                    ((HitBoxInteractable)o.o).moveThis(parseCoordinate(command[2], true), parseCoordinate(command[3], false));
                    break;
                case CSLObjectType.movingplatform:
                case CSLObjectType.rust:
                case CSLObjectType.slope:
                case CSLObjectType.wall:
                    ((LevelBlock)o.o)._X = parseCoordinate(command[2], true);
                    ((LevelBlock)o.o)._Y = parseCoordinate(command[3], false);
                    break;
                default:
                    ((GameObject)o.o).moveThis(parseCoordinate(command[2], true), parseCoordinate(command[3], false));
                    break;
            }
        }
        public void parseMoveCamera()
        {
            gameState.cameraPosition.X = parseCoordinateInt(command[1], true);
            gameState.cameraPosition.X = parseCoordinateInt(command[2], false);
            if(commands.Count > 3)
                gameState.cameraScale = float.Parse(command[3]);
        }
        public void parseSlideCamera()
        {
            events.Add(new SlideCameraEvent(gameState, level, this, int.Parse(command[1]), int.Parse(command[2]), double.Parse(command[3])));
        }
        public void parseTakePlayerControl()
        {
            gameState.acceptPlayerInput = false;
        }
        public void parseWaitForInstruction()
        {
            needsNextCommand = false;
            waitingForCommand = true;
        }
        public void parseWaitTime()
        {
            events.Add(new WaitEvent(gameState, level, this, double.Parse(command[1])));
            parseWaitForInstruction();
        }
        public void parseWalk()
        {
            Boolean b = true;
            if (command[5] == "false")
                b = false;
            events.Add(new WalkEvent(gameState, level, this, ((ScriptedObject)objects[command[1]]).type, ((ScriptedObject)objects[command[1]]).o, parseCoordinateVector(command[2], command[3]), double.Parse(command[4]), b));
        }
        private Point parsePointFromVector(Vector2 v)
        {
            return new Point((int)v.X, (int)v.Y);
        }
        private Vector2 parseCoordinateVector(string x, string y)
        {
            Vector2 vec = new Vector2();
            vec.X = parseCoordinate(x, true);
            vec.Y = parseCoordinate(y, false);
            return vec;
        }
        private BoundingBox parseHitBox(string x, string y, string width, string height)
        {
            BoundingBox b = new BoundingBox();
            b.Min.X = parseCoordinate(x, true);
            b.Min.Y = parseCoordinate(y, false);
            b.Max.X = b.Min.X + float.Parse(width);
            b.Max.Y = b.Min.Y + float.Parse(height);
            return b;
        }
        private float parseCoordinate(string x, Boolean isX)
        {
            float camPos;
            if(isX)
                camPos = gameState.cameraPosition.X;
            else
                camPos = gameState.cameraPosition.Y;
            if (x.StartsWith("c"))
                return float.Parse(x.Substring(1)) + camPos;
            else
                return float.Parse(x);
        }
        private int parseCoordinateInt(string x, Boolean isX)
        {
            int camPos;
            if (isX)
                camPos = gameState.cameraPosition.X;
            else
                camPos = gameState.cameraPosition.Y;
            if (x.StartsWith("c"))
                return int.Parse(x.Substring(1)) + camPos;
            else
                return int.Parse(x);
        }
        public void execute()
        {
            if (!hasExecuted)
            {
                hasExecuted = true;
                needsNextCommand = true;
            }
        }
        public void end()
        {
            needsNextCommand = false;
        }
        public void go(int i)
        {
            selectedIndex = i;
        }
        public void reset()
        {
            selectedIndex = 0;
            needsNextCommand = false;
        }
        public void addCleanupEvent(HandledEvent e)
        {
            cleanup.Add(e);
        }
        public void cleanupEvents()
        {
            while (cleanup.Count != 0)
            {
                events.Remove(cleanup[0]);
                cleanup.RemoveAt(0);
            }
        }
        public void drawThis(drawPacket pack)
        {
            foreach (HandledEvent e in events)
                e.drawThis(pack);
        }
    }
    public class ScriptedObject
    {
        public Object o;
        public CSLObjectType type;
        public ScriptedObject(Object o, CSLObjectType type)
        {
            this.o = o;
            this.type = type;
        }
    }
}

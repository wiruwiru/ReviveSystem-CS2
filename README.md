# ReviveSystem CS2
> Allows players to revive one of their teammates per round by holding “E” on the dead teammate.. (for CounterStrikeSharp)

### 💖 Revive preview
https://github.com/user-attachments/assets/f197f07f-ed26-4580-9e37-e85e86607e06

### ✋ Revive limit
https://github.com/user-attachments/assets/9f2813f7-a885-4289-8593-d2c87a6321d5

## ⚙️ Installation
1. Install [CounterStrike Sharp](https://github.com/roflmuffin/CounterStrikeSharp) and [Metamod:Source](https://www.sourcemm.net/downloads.php/?branch=master).

2. Download [ReviveSystem.zip](https://github.com/wiruwiru/ReviveSystem-CS2/releases) from the releases section.

3. Unzip the archive and upload it to the game server.

4. Start the server and wait for the `config.json` file to be generated.

5. Complete the configuration file with the parameters of your choice.

## 📁 Configuration
The configuration file (`ReviveSystem.json`) will be auto-generated after the first launch.

| Parameter            | Description                                                                                       | Default |
|----------------------|---------------------------------------------------------------------------------------------------|----------|
| `CanReviveOthersFlag` | Permission needed for a player to revive others. Leave empty to allow all players. | **`@css/vip`** |
| `CanBeRevivedFlag` | Permission needed to revive a player. Leave empty to allow reviving any dead teammate. | **`empty`** |
| `ReviveTime` | Time (in seconds) a player must hold the USE key to complete the revive. | **`15`** |
| `ReviveRange` | Maximum distance between the reviver and the corpse to allow reviving. | **`100.0f`** |
| `MaxRevivesPerRound` | Maximum number of revives a player can perform per round. | **`1`** |

## 📜 Lang Configuration
In the 'lang' folder, you'll find various files. For instance, 'es.json' is designated for the Spanish language. You're welcome to modify this file to better suit your style and language preferences. The language utilized depends on your settings in 'core.json' of CounterStrikeSharp.

## TO-DO  
The list of upcoming tasks and features has been moved to a dedicated file. You can find it here: [TO-DO.md](TODO.md)  

## 💡 Credits
- Thanks to [daffyyyy](https://github.com/daffyyyy) for the respawn function from their plugin [CS2-SimpleAdmin](https://github.com/daffyyyy/CS2-SimpleAdmin), which is used in this project.
- Thanks to [zakriamansoor47](https://github.com/zakriamansoor47) for the beacon function from their plugin [SLAYER_Duel](https://github.com/zakriamansoor47/SLAYER_Duel), which is used in this project.

## 📄 License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
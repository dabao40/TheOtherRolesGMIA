#From Dabao40, The Other Roles GMIA, the first version

import random

Improles = ["Neko-Kabocha", "Evil Swapper", "Evil Tracker", "Evil Lighter", "Serial Killer", 
            "Ninja", "Evil Watcher", "Eraser", "Morphling",
 "Camouflager", "Vampire", "Trickster", "Manipulator", "Bounty Hunter", "Witch", "Evil Mini", 
 "Evil Guesser", "Bomber", "Evil Hacker", "Impostor", "Warlock", "Ghosthjt", "Cleaner", "Godfather", 
 "Assasin", "Mimic (Killer)", "Flashlight", "Door Hacker", "Blackmailer", "Evil Yasuna", 
 "Killer Creator"]

imp_roles = []
crew_roles = []
neu_roles = []

for i in range (40):
    imp_roles.append(" ")

#imp_roles refers to the roles of the Imps
#num_imp refers to the number of Imps
def get_imp_role(imp_roles, num_imp):
    random.shuffle(Improles)
    for i in range (num_imp):
        
        imp_roles[i] = Improles[i]
        
        if Improles[i] == "Godfather":
            if num_imp == 3:
                #append all the Mafia roles
                imp_roles[0] = "Godfather"
                imp_roles[1] = "Mafioso"
                imp_roles[2] = "Janitor"
                
                return imp_roles
                
        elif Improles[i] == "Mimic (Killer)":
            if num_imp >= 2:
                #append the Assistant too
                imp_roles[0] = "Mimic (Killer)"
                imp_roles[1] = "Mimic (Assistant)"

    return imp_roles

number_imps = int(input("Please type the number of Impostors\n"))

get_imp_role(imp_roles, number_imps)
for i in range (number_imps):
    print(imp_roles[i])
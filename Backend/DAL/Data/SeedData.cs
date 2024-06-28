using DAL.Entities;
using DAL.Repositories;
namespace DAL.Models;

public class SeedData(
    InterestsRepository _interestsRepository,
    PicturesRepository _picturesRepository,
    ProfilesRepository _profilesRepository,
    UsersRepository _usersRepository,
    UsersInterestsRepository _usersInterestsRepository
    )
{
    public async Task Seed()
    {
            string[] biographies = new string[]
            {
                "Loyal to Anakin, veteran of numerous battles, resisted Order 66, joined Rebel Alliance, renowned for strategic brilliance.",
                "Obi-Wan's right hand, highly disciplined, executed Order 66, struggled with loyalty, later sought redemption.",
                "ARC trooper, uncovered Order 66 conspiracy, labeled a traitor, died trying to reveal truth, remembered as a hero.",
                "Tech-savvy ARC trooper, presumed dead, later rescued by Bad Batch, enhanced with cybernetics, key to Clone Force 99.",
                "Plo Koon's trusted commander, tactical genius, removed inhibitor chip, survived Galactic Empire, aided early Rebellion efforts.",
                "Commando, lost memory, rediscovered by D-Squad, eccentric but skilled, joined Rex and Wolffe in rebellion against Empire.",
                "Experienced inexplicable behavior, inadvertently revealed inhibitor chip's purpose, tragic figure highlighting clones' lack of autonomy.",
                "Zealous by-the-book soldier, initially sided against Krell, later regretted actions, symbol of clones' internal conflicts.",
                "Brave, impulsive, sacrificed himself to destroy Separatist supply ship, remembered for valor and dedication to his brothers.",
                "ARC trooper, loyal to Republic, conflicted during Order 66, perished in final battles, epitomized clones' tragic fate.",
                "A skilled ARC trooper, renowned for his strategic brilliance and unwavering loyalty, played a pivotal role in numerous key battles throughout the Clone Wars.",
                "As a sharpshooter in the 501st Legion, his exceptional marksmanship made him a critical asset in many high-stakes missions.",
                "A veteran sergeant with unmatched combat experience, he led his squad with courage and tactical acumen through some of the most challenging campaigns.",
                "Known for his calm under fire, this medic saved countless lives on the battlefield, earning the respect and admiration of his fellow troopers.",
                "A demolitions expert, he specialized in breaching enemy defenses, using his technical skills to turn the tide in critical engagements.",
                "This reconnaissance trooper’s expertise in gathering intelligence and scouting enemy positions was crucial for planning successful assaults.",
                "A loyal clone commander, he was deeply trusted by his Jedi General, executing complex strategies with precision and dedication.",
                "A heavy weapons specialist, his prowess with blaster cannons and grenade launchers provided crucial firepower in numerous skirmishes.",
                "An engineer, adept at repairing vehicles and equipment under fire, ensuring his unit remained operational during intense combat.",
                "As an elite pilot, he executed daring maneuvers and provided critical air support during pivotal space battles.",
                "This clone commando excelled in covert operations, utilizing stealth and precision to accomplish high-risk missions behind enemy lines.",
                "A sniper, his long-range shooting skills neutralized key enemy targets, safeguarding his squad from ambushes.",
                "An explosives technician, his mastery of ordnance allowed for strategic demolitions and sabotage of enemy facilities.",
                "Known for his exceptional leadership, he commanded respect and loyalty, leading his battalion to victory through numerous difficult engagements.",
                "This communications officer ensured seamless coordination between ground forces and command, vital for orchestrating successful operations.",
                "A decorated clone captain, he was instrumental in leading assaults and defending key positions against overwhelming odds.",
                "This strategist was known for his ability to outthink the enemy, devising innovative tactics that often led to decisive victories.",
                "As a frontline trooper, his bravery and tenacity in battle inspired his comrades and struck fear into the hearts of foes.",
                "A clone intelligence officer, his keen analytical mind and resourcefulness uncovered critical enemy plans and strategies.",
                "This reconnaissance specialist’s ability to navigate hostile terrain and report enemy movements was key to many ambushes and counterattacks.",
                "A combat instructor, he trained new recruits in advanced warfare techniques, ensuring they were battle-ready.",
                "As a heavy trooper, his imposing presence and weaponry provided critical suppression fire in heated engagements.",
                "This scout trooper’s agility and speed made him an excellent tracker, adept at pursuing and capturing enemy operatives.",
                "Known for his exceptional combat skills, he excelled in close-quarters battle, often turning the tide in tight situations.",
                "A field medic, his quick thinking and medical expertise saved numerous lives, earning him the undying gratitude of his comrades.",
                "This veteran trooper had a knack for survival, often pulling through the direst situations with resilience and resourcefulness.",
                "As a tactical officer, his planning and coordination were key to executing complex operations with precision and success.",
                "A veteran of countless battles, his experience and wisdom were invaluable to his unit, guiding them through the war’s many challenges.",
                "This demolitions expert’s ability to handle volatile explosives under pressure was critical in disabling enemy fortifications.",
                "A specialist in urban combat, his skills in navigating and fighting in dense environments were unmatched.",
                "As a commando, his proficiency in unconventional warfare made him a crucial asset in high-risk missions.",
                "This veteran sniper’s marksmanship was legendary, often making the difference in critical engagements.",
                "Known for his bravery, he led countless charges into enemy lines, rallying his fellow troopers with his fearless example.",
                "This clone commander’s strategic insight and leadership were instrumental in several major Republic victories.",
                "As a field engineer, his ability to construct and repair fortifications under fire was vital to his unit’s defense.",
                "An elite pilot, his skill in aerial dogfights and precision bombing runs provided essential support to ground forces.",
                "This intelligence operative’s espionage and counterintelligence efforts were key in thwarting enemy plots and assassinations.",
                "A frontline medic, his dedication to saving lives under fire made him a hero among his peers.",
                "Known for his unwavering loyalty, he was a steadfast soldier, always placing the mission and his comrades above himself.",
                "As a heavy weapons expert, his firepower was often the deciding factor in repelling enemy assaults."
            };
            string[] interestsData = new string[]
            {
                "Lightsaber_duels",
                "Blasters_and_marksmanship",
                "Imperial_fleet_tactics",
                "Stormtrooper_armor_customization",
                "Death_Star_blueprints",
                "TIE_Fighter_piloting",
                "Sith_Lord_fan_clubs",
                "Droid_repair_workshops",
                "HoloChess_tournaments",
                "Imperial_propaganda_analysis",
                "Womp_rat_hunting_competitions",
                "Rancor_whispering",
                "Jedi_mind_trick_appreciation",
                "Clone_Wars_reenactments",
                "Hutt_cuisine_tasting",
                "Mos_Eisley_cantina_karaoke",
                "Ewok_cultural_exchange",
                "Sarlacc_pit_diving",
                "Mandalorian_armor_crafting",
                "Bounty_hunting_escapades",
                "Nerf_herding_techniques",
                "Wookiee_language_proficiency",
                "Gamorrean_guard_weightlifting",
                "Podracing_enthusiasts",
                "Holocron_collecting",
                "First_Order_conspiracy_theories",
                "Rebel_Alliance_infiltration_strategies",
                "Saber_crystal_hunting",
                "Speeder_bike_racing",
                "Huttball_league_championships",
                "Jabba's_palace_dance-offs",
                "Swoop_bike_modifications",
                "Jawa_scavenging_expeditions",
                "Cloud_City_sabacc_tournaments",
                "Battlefront_VR_simulations",
                "Wampa_ice_cave_spelunking",
                "Sith_alchemy_hobbyists",
                "Ewok_village_architecture_tours",
                "Cantina_cocktail_mixology",
                "Hyperspace_navigation_seminars",
                "HoloNet_meme_creation",
                "Gungan_underwater_exploration",
                "Astro_droid_dance_parties",
                "Geonosian_hive_mind_studies",
                "Dewback_riding_adventures",
                "Sandcrawler_restoration_projects",
                "Dianoga_aquarium_keeping",
                "Kyber_crystal_meditation",
                "Hoth_ice_sculpting_contests"
            };
            Random random = new Random();

            var users = new List<User>();
            var Profiles = new List<Profile>();
            var pictures = new List<Picture>();
            var interests = new List<Interest>();
            var userInterests = new List<UserInterest>();

            for (var i = 1; i <= interestsData.Length; i++)
            {
                var interest = new Interest()
                {
                    InterestId = i,
                    Name = interestsData[i - 1]
                };
                interests.Add(interest);
            }

            foreach (var interest in interests)
            {
                await _interestsRepository.CreateInterestWithIdAsync(interest.InterestId, interest.Name);
            }

            for (var i = 1; i <= 500; i++)
            {
                int numberOfInterests = random.Next(1, 6);
                var interestIds = new HashSet<int>();
                while (interestIds.Count < numberOfInterests)
                {
                    interestIds.Add(random.Next(1, interestsData.Length));
                }

                foreach (var interestId in interestIds)
                {
                    var userInterest = new UserInterest()
                    {
                        UserId = i,
                        InterestId = interestId
                    };
                    userInterests.Add(userInterest);
                }
            }

            foreach (var userInterest in userInterests)
            {
                await _usersInterestsRepository.AddUserInterestAsync(userInterest);
            }

            string projectDirectory = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName;
            string maleFolderPath = Path.Combine(projectDirectory, "Matcha_Data", "male");
            string femaleFolderPath = Path.Combine(projectDirectory, "Matcha_Data", "female");
            string[] maleImageNames = Directory.GetFiles(maleFolderPath);
            string[] femaleImageNames = Directory.GetFiles(femaleFolderPath);
            var maleImages = new List<byte[]>();
            var femaleImages = new List<byte[]>();
            foreach (var name in maleImageNames)
            {
                maleImages.Add(File.ReadAllBytes(name));
            }

            foreach (var name in femaleImageNames)
            {
                femaleImages.Add(File.ReadAllBytes(name));
            }

            int maleImagesCount = maleImages.Count;
            int femaleImagesCount = femaleImages.Count;
            int maleImageIndex = 0;
            int femaleImageIndex = 0;

            for (var i = 1; i <= 500; i++)
            {
                var picture = new Picture()
                {
                    Id = i,
                    UserId = i,
                    PicturePath = i % 2 == 0
                        ? maleImages[maleImageIndex]
                        : femaleImages[femaleImageIndex],
                    IsProfilePicture = true
                };
                pictures.Add(picture);

                // Increment the counters and reset them if they reach the end of the list
                if (i % 2 == 0)
                {
                    maleImageIndex = (maleImageIndex + 1) % maleImagesCount;
                }
                else
                {
                    femaleImageIndex = (femaleImageIndex + 1) % femaleImagesCount;
                }
            }

            foreach (var picture in pictures)
            {
                await _picturesRepository.UploadWithIdPhoto(picture.Id, picture.UserId, picture.PicturePath, 1);
            }

            for (var i = 1; i <= 500; i++)
            {
                var bioIndex = random.Next(biographies.Length);
                var gender = i % 2 == 0 ? "male" : "female";
                var profile = new Profile()
                {
                    Id = i,
                    Gender = gender,
                    Age = random.Next(18, 50),
                    IsActive = true,
                    Latitude = random.Next(-80, 80),
                    Longitude = random.Next(-180, 180),
                    Biography = biographies[bioIndex],
                    FameRating = i,
                    ProfilePictureId = i,
                    SexualPreferences = i % 3 == 0 ? "male" : "female"
                };
                Profiles.Add(profile);
            }

            foreach (var profile in Profiles)
            {
                await _profilesRepository.CreateProfileWithIdAsync(profile);
            }

            for (var i = 1; i <= 500; i++)
            {
                var user = new User()
                {
                    Id = i,
                    UserName = "CloneTrooper" + i,
                    FirstName = "Clone",
                    LastName = "Trooper",
                    Email = "Clone trooper" + i + "@gmail.com",
                    Password = HashPassword("Clone123!"),
                    IsVerified = true,
                };
                users.Add(user);
            }

            foreach (var user in users)
            {
                await _usersRepository.CreateUserWithIdAsync(user);
            }
    }

    private static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}
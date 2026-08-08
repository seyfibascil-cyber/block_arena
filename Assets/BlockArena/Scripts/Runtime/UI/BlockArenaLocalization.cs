using System;
using System.Collections.Generic;
using UnityEngine;

public static class BlockArenaLocalization
{
    public const string LanguagePreferenceKey = "BlockArena.Settings.Language";

    private static readonly Dictionary<string, string[]> Entries =
        new Dictionary<string, string[]>(StringComparer.Ordinal)
        {
            { "quick_play", V("HIZLI\nOYNA", "QUICK\nPLAY", "PARTIDA\nRÁPIDA", "SCHNELL\nSPIELEN", "PARTIE\nRAPIDE", "PARTITA\nRAPIDA", "БЫСТРАЯ\nИГРА", "لعب\nسريع", "クイック\nプレイ") },
            { "pvp", V("PVP", "PVP", "PVP", "PVP", "PVP", "PVP", "PVP", "PVP", "PVP") },
            { "levels", V("BÖLÜMLER", "LEVELS", "NIVELES", "LEVEL", "NIVEAUX", "LIVELLI", "УРОВНИ", "المراحل", "レベル") },
            { "characters", V("KARAKTERLER", "CHARACTERS", "PERSONAJES", "FIGUREN", "PERSONNAGES", "PERSONAGGI", "ПЕРСОНАЖИ", "الشخصيات", "キャラクター") },
            { "missions", V("GÖREVLER", "MISSIONS", "MISIONES", "AUFGABEN", "MISSIONS", "MISSIONI", "ЗАДАНИЯ", "المهام", "ミッション") },
            { "settings", V("AYARLAR", "SETTINGS", "AJUSTES", "EINSTELLUNGEN", "RÉGLAGES", "IMPOSTAZIONI", "НАСТРОЙКИ", "الإعدادات", "設定") },
            { "back", V("GERİ", "BACK", "VOLVER", "ZURÜCK", "RETOUR", "INDIETRO", "НАЗАД", "رجوع", "戻る") },
            { "on", V("AÇIK", "ON", "SÍ", "AN", "OUI", "SÌ", "ВКЛ", "تشغيل", "オン") },
            { "off", V("KAPALI", "OFF", "NO", "AUS", "NON", "NO", "ВЫКЛ", "إيقاف", "オフ") },
            { "music", V("MÜZİK", "MUSIC", "MÚSICA", "MUSIK", "MUSIQUE", "MUSICA", "МУЗЫКА", "الموسيقى", "音楽") },
            { "sound_effects", V("SES EFEKTLERİ", "SOUND EFFECTS", "EFECTOS", "SOUNDEFFEKTE", "EFFETS SONORES", "EFFETTI SONORI", "ЗВУКИ", "المؤثرات", "効果音") },
            { "vibration", V("TİTREŞİM", "VIBRATION", "VIBRACIÓN", "VIBRATION", "VIBRATION", "VIBRAZIONE", "ВИБРАЦИЯ", "الاهتزاز", "振動") },
            { "connect", V("BAĞLAN", "CONNECT", "CONECTAR", "VERBINDEN", "CONNECTER", "COLLEGA", "ПОДКЛЮЧИТЬ", "اتصال", "接続") },
            { "language", V("DİL", "LANGUAGE", "IDIOMA", "SPRACHE", "LANGUE", "LINGUA", "ЯЗЫК", "اللغة", "言語") },
            { "how_to_play", V("NASIL OYNANIR?", "HOW TO PLAY", "CÓMO JUGAR", "SPIELANLEITUNG", "COMMENT JOUER", "COME SI GIOCA", "КАК ИГРАТЬ", "طريقة اللعب", "遊び方") },
            { "parent_guide", V("EBEVEYN KILAVUZU", "PARENT GUIDE", "GUÍA PARA PADRES", "ELTERNRATGEBER", "GUIDE PARENTAL", "GUIDA GENITORI", "ДЛЯ РОДИТЕЛЕЙ", "دليل الوالدين", "保護者ガイド") },
            { "support", V("YARDIM VE DESTEK", "HELP & SUPPORT", "AYUDA Y SOPORTE", "HILFE & SUPPORT", "AIDE ET SUPPORT", "AIUTO E SUPPORTO", "ПОМОЩЬ", "المساعدة والدعم", "ヘルプ") },
            { "credits", V("HAZIRLAYANLAR", "CREDITS", "CRÉDITOS", "MITWIRKENDE", "CRÉDITS", "CREDITI", "АВТОРЫ", "فريق العمل", "クレジット") },
            { "privacy", V("GİZLİLİK", "PRIVACY", "PRIVACIDAD", "DATENSCHUTZ", "CONFIDENTIALITÉ", "PRIVACY", "КОНФИДЕНЦИАЛЬНОСТЬ", "الخصوصية", "プライバシー") },
            { "terms", V("KULLANIM ŞARTLARI", "TERMS OF USE", "TÉRMINOS DE USO", "NUTZUNGSBEDINGUNGEN", "CONDITIONS D’UTILISATION", "TERMINI DI UTILIZZO", "УСЛОВИЯ", "شروط الاستخدام", "利用規約") },
            { "move", V("HAREKET ET", "MOVE", "MUÉVETE", "BEWEGEN", "DÉPLACE-TOI", "MUOVITI", "ХОД", "تحرك", "移動") },
            { "place_obstacle", V("ENGEL YERLEŞTİR", "PLACE AN OBSTACLE", "COLOCA UN OBSTÁCULO", "HINDERNIS SETZEN", "PLACE UN OBSTACLE", "POSIZIONA OSTACOLO", "ПОСТАВЬ ПРЕГРАДУ", "ضع عائقًا", "障害物を置く") },
            { "trap_rival", V("RAKİBİ KAPAT", "TRAP YOUR RIVAL", "ATRAPA AL RIVAL", "GEGNER EINKREISEN", "BLOQUE LE RIVAL", "BLOCCA IL RIVALE", "ЗАПРИ СОПЕРНИКА", "حاصر خصمك", "相手を囲む") },
            { "move_help", V("Karakterini yeşil karelerden birine taşı.", "Move your character to a green tile.", "Mueve tu personaje a una casilla verde.", "Bewege deine Figur auf ein grünes Feld.", "Déplace ton personnage sur une case verte.", "Sposta il personaggio su una casella verde.", "Перемести героя на зелёную клетку.", "حرّك شخصيتك إلى مربع أخضر.", "緑のマスへ移動しよう。") },
            { "obstacle_help", V("Sonra kırmızı karelerden birine engel koy.", "Then place an obstacle on a red tile.", "Después coloca un obstáculo en una casilla roja.", "Setze dann ein Hindernis auf ein rotes Feld.", "Place ensuite un obstacle sur une case rouge.", "Poi metti un ostacolo su una casella rossa.", "Затем поставь преграду на красную клетку.", "ثم ضع عائقًا على مربع أحمر.", "次に赤いマスへ障害物を置こう。") },
            { "trap_help", V("Rakibin gidecek yeri kalmadığında kazanırsın!", "You win when your rival has nowhere to move!", "¡Ganas cuando tu rival no puede moverse!", "Du gewinnst, wenn der Gegner nicht mehr ziehen kann!", "Tu gagnes quand ton rival ne peut plus bouger !", "Vinci quando il rivale non può più muoversi!", "Ты победишь, когда сопернику некуда ходить!", "تفوز عندما لا يجد خصمك مكانًا للتحرك!", "相手が動けなくなれば勝ち！") },
            { "easy", V("KOLAY", "EASY", "FÁCIL", "LEICHT", "FACILE", "FACILE", "ЛЕГКО", "سهل", "かんたん") },
            { "medium", V("ORTA", "MEDIUM", "MEDIO", "MITTEL", "MOYEN", "MEDIO", "СРЕДНЕ", "متوسط", "ふつう") },
            { "hard", V("ZOR", "HARD", "DIFÍCIL", "SCHWER", "DIFFICILE", "DIFFICILE", "СЛОЖНО", "صعب", "むずかしい") },
            { "impossible", V("İMKÂNSIZ", "IMPOSSIBLE", "IMPOSIBLE", "UNMÖGLICH", "IMPOSSIBLE", "IMPOSSIBILE", "НЕВОЗМОЖНО", "مستحيل", "不可能") },
            { "choose_difficulty", V("ZORLUK SEÇ", "CHOOSE DIFFICULTY", "ELIGE DIFICULTAD", "SCHWIERIGKEIT WÄHLEN", "CHOISIR LA DIFFICULTÉ", "SCEGLI DIFFICOLTÀ", "ВЫБЕРИ СЛОЖНОСТЬ", "اختر الصعوبة", "難易度を選ぶ") },
            { "your_turn", V("SENİN SIRAN", "YOUR TURN", "TU TURNO", "DU BIST DRAN", "À TON TOUR", "TOCCA A TE", "ТВОЙ ХОД", "دورك", "あなたの番") },
            { "rival_thinking", V("RAKİP DÜŞÜNÜYOR...", "RIVAL IS THINKING...", "EL RIVAL PIENSA...", "GEGNER DENKT...", "LE RIVAL RÉFLÉCHIT...", "IL RIVALE PENSA...", "СОПЕРНИК ДУМАЕТ...", "الخصم يفكر...", "相手が考え中…") },
            { "you_won", V("KAZANDIN!", "YOU WON!", "¡GANASTE!", "GEWONNEN!", "GAGNÉ !", "HAI VINTO!", "ПОБЕДА!", "فزت!", "勝利！") },
            { "you_lost", V("KAYBETTİN!", "YOU LOST!", "PERDISTE", "VERLOREN!", "PERDU !", "HAI PERSO!", "ПОРАЖЕНИЕ", "خسرت!", "敗北") },
            { "next_level", V("SONRAKİ BÖLÜM", "NEXT LEVEL", "SIGUIENTE NIVEL", "NÄCHSTES LEVEL", "NIVEAU SUIVANT", "LIVELLO SUCCESSIVO", "СЛЕДУЮЩИЙ УРОВЕНЬ", "المرحلة التالية", "次のレベル") },
            { "retry", V("TEKRAR OYNA", "PLAY AGAIN", "JUGAR DE NUEVO", "NOCH EINMAL", "REJOUER", "GIOCA ANCORA", "ЕЩЁ РАЗ", "العب مجددًا", "もう一度") },
            { "main_menu", V("ANA MENÜ", "MAIN MENU", "MENÚ PRINCIPAL", "HAUPTMENÜ", "MENU PRINCIPAL", "MENU PRINCIPALE", "ГЛАВНОЕ МЕНЮ", "القائمة الرئيسية", "メインメニュー") },
            { "understood", V("ANLADIM", "GOT IT", "ENTENDIDO", "VERSTANDEN", "COMPRIS", "CAPITO", "ПОНЯТНО", "فهمت", "わかった") },
            { "return_levels", V("BÖLÜMLERE DÖN", "BACK TO LEVELS", "VOLVER A NIVELES", "ZURÜCK ZU LEVELS", "RETOUR AUX NIVEAUX", "TORNA AI LIVELLI", "К УРОВНЯМ", "العودة للمراحل", "レベルへ戻る") },
            { "leave_prompt", V("OYUNDAN ÇIKMAK İSTEDİĞİNE EMİN MİSİN?", "ARE YOU SURE YOU WANT TO LEAVE?", "¿SEGURO QUE QUIERES SALIR?", "MÖCHTEST DU WIRKLICH GEHEN?", "VEUX-TU VRAIMENT QUITTER ?", "VUOI DAVVERO USCIRE?", "ТОЧНО ВЫЙТИ ИЗ ИГРЫ?", "هل تريد الخروج من اللعبة؟", "ゲームを終了しますか？") },
            { "yes_leave", V("EVET, ÇIK", "YES, LEAVE", "SÍ, SALIR", "JA, VERLASSEN", "OUI, QUITTER", "SÌ, ESCI", "ДА, ВЫЙТИ", "نعم، خروج", "はい、終了") },
            { "cancel", V("VAZGEÇ", "CANCEL", "CANCELAR", "ABBRECHEN", "ANNULER", "ANNULLA", "ОТМЕНА", "إلغاء", "キャンセル") },
            { "level", V("BÖLÜM", "LEVEL", "NIVEL", "LEVEL", "NIVEAU", "LIVELLO", "УРОВЕНЬ", "مرحلة", "レベル") },
            { "arena_start", V("BAŞLANGIÇ ARENASI", "STARTER ARENA", "ARENA INICIAL", "STARTARENA", "ARÈNE DE DÉPART", "ARENA INIZIALE", "НАЧАЛЬНАЯ АРЕНА", "ساحة البداية", "スタートアリーナ") },
            { "arena_forest", V("ORMAN", "FOREST", "BOSQUE", "WALD", "FORÊT", "FORESTA", "ЛЕС", "الغابة", "森") },
            { "arena_ice", V("BUZ DÜNYASI", "ICE WORLD", "MUNDO DE HIELO", "EISWELT", "MONDE DE GLACE", "MONDO DI GHIACCIO", "ЛЕДЯНОЙ МИР", "عالم الجليد", "氷の世界") },
            { "arena_lava", V("LAV ARENASI", "LAVA ARENA", "ARENA DE LAVA", "LAVA-ARENA", "ARÈNE DE LAVE", "ARENA DI LAVA", "ЛАВОВАЯ АРЕНА", "ساحة الحمم", "溶岩アリーナ") },
            { "arena_space", V("UZAY ARENASI", "SPACE ARENA", "ARENA ESPACIAL", "WELTRAUMARENA", "ARÈNE SPATIALE", "ARENA SPAZIALE", "КОСМИЧЕСКАЯ АРЕНА", "ساحة الفضاء", "宇宙アリーナ") },
            { "coin", V("JETON", "COIN", "MONEDA", "MÜNZE", "PIÈCE", "MONETA", "МОНЕТА", "عملة", "コイン") },
            { "star", V("YILDIZ", "STAR", "ESTRELLA", "STERN", "ÉTOILE", "STELLA", "ЗВЕЗДА", "نجمة", "スター") },
            { "selected", V("SEÇİLİ", "SELECTED", "ELEGIDO", "GEWÄHLT", "SÉLECTIONNÉ", "SELEZIONATO", "ВЫБРАНО", "محدد", "選択中") },
            { "select", V("SEÇ", "SELECT", "ELEGIR", "WÄHLEN", "CHOISIR", "SCEGLI", "ВЫБРАТЬ", "اختر", "選ぶ") },
            { "locked", V("KİLİTLİ", "LOCKED", "BLOQUEADO", "GESPERRT", "VERROUILLÉ", "BLOCCATO", "ЗАКРЫТО", "مغلق", "ロック中") },
            { "claimed", V("ALINDI", "CLAIMED", "RECOGIDO", "ABGEHOLT", "RÉCUPÉRÉ", "RISCATTATO", "ПОЛУЧЕНО", "تم الاستلام", "受取済み") },
            { "claim", V("AL", "CLAIM", "RECOGER", "ABHOLEN", "RÉCUPÉRER", "RITIRA", "ПОЛУЧИТЬ", "استلام", "受け取る") },
            { "mission_play", V("3 MAÇ TAMAMLA", "COMPLETE 3 MATCHES", "COMPLETA 3 PARTIDAS", "3 SPIELE ABSCHLIESSEN", "TERMINER 3 PARTIES", "COMPLETA 3 PARTITE", "ЗАВЕРШИ 3 МАТЧА", "أكمل 3 مباريات", "3試合プレイ") },
            { "mission_win", V("2 MAÇ KAZAN", "WIN 2 MATCHES", "GANA 2 PARTIDAS", "2 SPIELE GEWINNEN", "GAGNER 2 PARTIES", "VINCI 2 PARTITE", "ВЫИГРАЙ 2 МАТЧА", "اربح مباراتين", "2試合勝利") },
            { "mission_level", V("1 BÖLÜM KAZAN", "WIN 1 LEVEL", "GANA 1 NIVEL", "1 LEVEL GEWINNEN", "GAGNER 1 NIVEAU", "VINCI 1 LIVELLO", "ПРОЙДИ 1 УРОВЕНЬ", "افز بمرحلة واحدة", "1レベル勝利") },
            { "mission_obstacle", V("10 ENGEL YERLEŞTİR", "PLACE 10 OBSTACLES", "COLOCA 10 OBSTÁCULOS", "10 HINDERNISSE SETZEN", "PLACER 10 OBSTACLES", "POSIZIONA 10 OSTACOLI", "ПОСТАВЬ 10 ПРЕГРАД", "ضع 10 عوائق", "障害物を10個置く") },
            { "mission_chest", V("GÖREV SANDIĞI", "MISSION CHEST", "COFRE DE MISIONES", "AUFGABENTRUHE", "COFFRE DE MISSIONS", "FORZIERE MISSIONI", "СУНДУК ЗАДАНИЙ", "صندوق المهام", "ミッション宝箱") },
            { "claim_chest", V("SANDIĞI AL", "CLAIM CHEST", "RECOGER COFRE", "TRUHE ABHOLEN", "OUVRIR LE COFFRE", "RITIRA FORZIERE", "ЗАБРАТЬ СУНДУК", "استلم الصندوق", "宝箱を受け取る") },
            { "not_enough_coins", V("YETERLİ JETONUN YOK", "NOT ENOUGH COINS", "NO HAY SUFICIENTES MONEDAS", "NICHT GENUG MÜNZEN", "PAS ASSEZ DE PIÈCES", "MONETE INSUFFICIENTI", "НЕДОСТАТОЧНО МОНЕТ", "لا توجد عملات كافية", "コインが足りません") },
            { "search_player", V("OYUNCU ARA", "FIND PLAYER", "BUSCAR JUGADOR", "SPIELER SUCHEN", "CHERCHER JOUEUR", "CERCA GIOCATORE", "НАЙТИ ИГРОКА", "ابحث عن لاعب", "対戦相手を探す") },
            { "cancel_search", V("ARAMAYI İPTAL ET", "CANCEL SEARCH", "CANCELAR BÚSQUEDA", "SUCHE ABBRECHEN", "ANNULER", "ANNULLA RICERCA", "ОТМЕНИТЬ ПОИСК", "إلغاء البحث", "検索を中止") },
            { "search_again", V("TEKRAR OYUNCU ARA", "SEARCH AGAIN", "BUSCAR DE NUEVO", "ERNEUT SUCHEN", "RECHERCHER", "CERCA DI NUOVO", "ИСКАТЬ СНОВА", "ابحث مجددًا", "もう一度探す") },
            { "play_bot", V("MEGA BOTLA OYNA", "PLAY MEGA BOT", "JUGAR CON MEGA BOT", "GEGEN MEGA-BOT", "JOUER CONTRE MÉGA BOT", "GIOCA CON MEGA BOT", "ИГРАТЬ С МЕГА-БОТОМ", "العب ضد البوت", "メガボットと対戦") },
            { "searching", V("GERÇEK OYUNCU ARANIYOR...", "SEARCHING FOR A REAL PLAYER...", "BUSCANDO JUGADOR REAL...", "ECHTER SPIELER GESUCHT...", "RECHERCHE D’UN JOUEUR...", "RICERCA GIOCATORE...", "ПОИСК ИГРОКА...", "جارٍ البحث عن لاعب...", "対戦相手を検索中…") },
            { "seconds", V("SANİYE", "SECONDS", "SEGUNDOS", "SEKUNDEN", "SECONDES", "SECONDI", "СЕКУНД", "ثانية", "秒") },
            { "real_rival", V("GERÇEK RAKİBE KARŞI OYNA", "PLAY A REAL RIVAL", "JUEGA CONTRA UN RIVAL", "GEGEN ECHTEN GEGNER", "JOUE CONTRE UN VRAI RIVAL", "GIOCA CONTRO UN RIVALE", "ИГРА С РЕАЛЬНЫМ СОПЕРНИКОМ", "العب ضد خصم حقيقي", "プレイヤーと対戦") },
            { "try_again", V("TEKRAR DENE", "TRY AGAIN", "REINTENTAR", "ERNEUT VERSUCHEN", "RÉESSAYER", "RIPROVA", "ПОВТОРИТЬ", "حاول مجددًا", "再試行") },
            { "connection_failed", V("BAĞLANTI KURULAMADI", "CONNECTION FAILED", "FALLO DE CONEXIÓN", "VERBINDUNG FEHLGESCHLAGEN", "ÉCHEC DE CONNEXION", "CONNESSIONE FALLITA", "ОШИБКА СОЕДИНЕНИЯ", "فشل الاتصال", "接続できません") },
            { "connecting", V("ÇEVRİM İÇİ SERVİSE BAĞLANILIYOR...", "CONNECTING TO ONLINE SERVICE...", "CONECTANDO AL SERVICIO...", "ONLINE-DIENST WIRD VERBUNDEN...", "CONNEXION AU SERVICE...", "CONNESSIONE AL SERVIZIO...", "ПОДКЛЮЧЕНИЕ...", "جارٍ الاتصال...", "オンライン接続中…") },
            { "player_found", V("GERÇEK OYUNCU BULUNDU", "REAL PLAYER FOUND", "JUGADOR ENCONTRADO", "SPIELER GEFUNDEN", "JOUEUR TROUVÉ", "GIOCATORE TROVATO", "ИГРОК НАЙДЕН", "تم العثور على لاعب", "対戦相手が見つかりました") },
            { "bot_ready", V("OYUNCU BULUNAMADI\nMEGA BOT HAZIR", "NO PLAYER FOUND\nMEGA BOT READY", "NO SE ENCONTRÓ JUGADOR\nMEGA BOT LISTO", "KEIN SPIELER GEFUNDEN\nMEGA-BOT BEREIT", "AUCUN JOUEUR TROUVÉ\nMÉGA BOT PRÊT", "NESSUN GIOCATORE\nMEGA BOT PRONTO", "ИГРОК НЕ НАЙДЕН\nМЕГА-БОТ ГОТОВ", "لم يتم العثور على لاعب\nالبوت جاهز", "相手が見つかりません\nメガボット準備完了") },
            { "match_failed", V("EŞLEŞTİRME BAŞARISIZ", "MATCHMAKING FAILED", "FALLO DE EMPAREJAMIENTO", "SPIELERSUCHE FEHLGESCHLAGEN", "ÉCHEC DU MATCHMAKING", "MATCHMAKING FALLITO", "ОШИБКА ПОДБОРА", "فشل العثور على خصم", "マッチング失敗") },
            { "parent_body", V("Block Arena sohbet içermez. PVP modunda gerçek oyuncu bulunamazsa bot kullanıldığı belirtilir. Ödüllü reklamlar isteğe bağlıdır.", "Block Arena has no chat. If no real player is found in PVP, the game clearly identifies the bot. Rewarded ads are optional.", "Block Arena no incluye chat. Si no se encuentra un jugador real en PVP, se indica claramente que se usa un bot. Los anuncios con recompensa son opcionales.", "Block Arena enthält keinen Chat. Wird im PVP kein echter Spieler gefunden, wird der Bot klar gekennzeichnet. Belohnungswerbung ist optional.", "Block Arena ne contient aucun chat. Si aucun joueur réel n’est trouvé en PVP, le bot est clairement indiqué. Les publicités récompensées sont facultatives.", "Block Arena non include chat. Se in PVP non viene trovato un giocatore reale, il bot viene indicato chiaramente. Gli annunci premio sono facoltativi.", "В Block Arena нет чата. Если в PVP не найден реальный игрок, бот будет явно обозначен. Реклама за награду необязательна.", "لا توجد محادثة في Block Arena. إذا لم يتم العثور على لاعب حقيقي في PVP، فسيتم توضيح استخدام البوت. إعلانات المكافآت اختيارية.", "Block Arenaにはチャットがありません。PVPで相手が見つからない場合は、ボットであることを明示します。リワード広告は任意です。") },
            { "support_body", V("Destek mesajınıza uygulama sürümünü ve cihaz modelinizi ekleyin.", "Include the app version and your device model in your support message.", "Incluye la versión de la aplicación y el modelo del dispositivo en tu mensaje.", "Gib in deiner Supportnachricht App-Version und Gerätemodell an.", "Indiquez la version de l’application et le modèle de votre appareil.", "Includi la versione dell’app e il modello del dispositivo nel messaggio.", "Укажите версию приложения и модель устройства.", "أضف إصدار التطبيق وطراز جهازك إلى رسالة الدعم.", "お問い合わせにはアプリのバージョンと端末名を記載してください。") },
            { "privacy_body", V("Oyun ilerlemesi cihazda saklanır. Reklam ve çevrimiçi hizmetlerin veri kullanımı gizlilik politikasında açıklanır.", "Game progress is stored on the device. Data use by ads and online services is explained in the privacy policy.", "El progreso se guarda en el dispositivo. El uso de datos por anuncios y servicios en línea se explica en la política de privacidad.", "Der Spielfortschritt wird auf dem Gerät gespeichert. Die Datennutzung durch Werbung und Onlinedienste wird in der Datenschutzrichtlinie erklärt.", "La progression est stockée sur l’appareil. L’utilisation des données est expliquée dans la politique de confidentialité.", "I progressi sono salvati sul dispositivo. L’uso dei dati è descritto nell’informativa sulla privacy.", "Прогресс хранится на устройстве. Использование данных описано в политике конфиденциальности.", "يتم حفظ تقدم اللعبة على الجهاز. يُشرح استخدام البيانات في سياسة الخصوصية.", "ゲームの進行状況は端末に保存されます。データ利用はプライバシーポリシーで説明します。") },
            { "terms_body", V("Block Arena eğlence amacıyla sunulur. Hile yapmak ve başkalarının oyun deneyimine zarar vermek yasaktır.", "Block Arena is provided for entertainment. Cheating or harming another player’s experience is prohibited.", "Block Arena se ofrece con fines de entretenimiento. Está prohibido hacer trampas o perjudicar la experiencia de otros.", "Block Arena dient der Unterhaltung. Cheaten oder das Spielerlebnis anderer zu beeinträchtigen ist verboten.", "Block Arena est proposé à des fins de divertissement. Tricher ou nuire à l’expérience d’autrui est interdit.", "Block Arena è offerto a scopo di intrattenimento. È vietato barare o danneggiare l’esperienza altrui.", "Block Arena предназначена для развлечения. Запрещено жульничать и мешать другим игрокам.", "تُقدم Block Arena للترفيه. يُحظر الغش أو الإضرار بتجربة الآخرين.", "Block Arenaは娯楽目的のゲームです。不正行為や他のプレイヤーへの迷惑行為は禁止です。") }
        };

    public static int CurrentLanguageIndex
    {
        get { return Mathf.Clamp(PlayerPrefs.GetInt(LanguagePreferenceKey, 0), 0, 8); }
    }

    public static string Text(string key)
    {
        string[] values;
        if (!Entries.TryGetValue(key, out values))
        {
            return key;
        }
        return values[CurrentLanguageIndex];
    }

    public static string Format(string key, params object[] args)
    {
        return string.Format(Text(key), args);
    }

    private static string[] V(
        string tr, string en, string es, string de, string fr,
        string it, string ru, string ar, string ja)
    {
        return new[] { tr, en, es, de, fr, it, ru, ar, ja };
    }
}

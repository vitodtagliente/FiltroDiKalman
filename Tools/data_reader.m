%timestamp dimensione pacch. AL dm K[1] K[2] P[1,1] P[1,2] P[2,1] P[2,2] sigma m C
S1='server.csv';%i file sono contenuti nel workspace di Matlab
S2='client.csv';
ReceiverRaw=csvread(S1);%dati letti dal file in formato csv
SenderRaw=csvread(S2);%vedi sopra
%estrapolo dalla matrice contenente tutti i dati letti dal file csv le
%singole colonne che mi rappresentano, una per una, i dati da rappresentare
Receiver_timestamp=ReceiverRaw(:,1)';
Receiver_AL=ReceiverRaw(:,2)';
Receiver_dm=ReceiverRaw(:,3)';
Receiver_K1=ReceiverRaw(:,4)';
Receiver_K2=ReceiverRaw(:,5)';
Receiver_P11=ReceiverRaw(:,6)';
Receiver_P12=ReceiverRaw(:,7)';
Receiver_P21=ReceiverRaw(:,8)';
Receiver_P22=ReceiverRaw(:,9)';
Receiver_sigma=ReceiverRaw(:,10)';
Receiver_m=ReceiverRaw(:,11)';
Receiver_C=ReceiverRaw(:,12)';
Sender_timestamp=SenderRaw(:,1)';
Sender_bitrate=SenderRaw(:,2)';
Receiver_timestamp=Receiver_timestamp-Receiver_timestamp(1,1);%normalizzazione dell'asse x dei tempi
Sender_timestamp=Sender_timestamp-Sender_timestamp(1,1);
clear S1 S2 ReceiverRaw SenderRaw;
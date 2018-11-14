#!/bin/ksh

#echo "sleep 60 ; " kill -9 `ps -ef |grep -i c179392 |grep -iE 'main1.sh|sshd' | grep -iv grep  | awk -F' ' '{print $2}'` | xargs echo > /tmp/ps2

#sh /tmp/ps2 &

##############################################################################################################################
# Author :- Shadab Tabish/Hemant Kumar
# Purpose of script :-
##############################################################################################################################

###############################################################
# Parameters initialization
###############################################################

directory_init=$1
user=$2
server=$3
key=$4
IAtooldir=/home/$2/IAToolLinuxService

directory="$(echo ${directory_init} | tr ',' ' ')"

###############################################################
# Validating user inputs (Directory)
###############################################################

if [ ! -d ${directory} ]; then



echo "Directory mentioned by user doesn't exist. Script terminated"
exit 0
fi

###############################################################
# Checking for existence of output file
###############################################################

cd ${IAtooldir}
NumberOfFiles=`find . -maxdepth 1 -name 'sam2.csv' | wc -l`
if [ ${NumberOfFiles} -ne 0 ]; then
rm -f ${IAtooldir}/sam2.csv
fi

###############################################################
# Finding files which contains keyword inside the file
###############################################################

if [ "${key} " != " " ]; then
#cd ${directory}
fn0=`grep -ilr ${key} ${directory} | grep -i \.sh$`
fn1=`grep -ilr ${key} ${directory} | grep -i \.nzsql$`
#echo "$fn" > tmp08761

echo "$fn0" > tmp456
echo "$fn1" >> tmp456

fn=`cat tmp456`



for f in $fn
do
echo `ls -ltr --time-style=+"%Y-%m-%d~%H:%M:%S" $f` >> ${IAtooldir}/sam1
done
fi

###############################################################
# Finding files which contains keyword in file name
###############################################################

#key_value='*.sh'

echo $key | grep -i '\.sh$' | wc -l

if [ 1 -eq `echo $key | grep -i '\.sh$' | wc -l` ]; then
fn2=`find ${directory} -iname "*'${key}'*"`
else
fn3=`find ${directory} -iname "*'${key}'*".sh`
fi

echo $key | grep -i '\.nzsql$' | wc -l

if [ 1 -eq `echo $key | grep -i '\.nzsql$' | wc -l` ]; then
fn2=`find ${directory} -iname "*'${key}'*"`
else
fn3=`find ${directory} -iname "*'${key}'*".nzsql`
fi

echo "$fn2" > tmp123
echo "$fn3" >> tmp123

fn4=`cat tmp123`
for f2 in $fn4
do
echo `ls -ltr --time-style=+"%Y-%m-%d~%H:%M:%S" $f2` >> ${IAtooldir}/sam1
done

###############################################################
#Removing intermediate files
###############################################################

cat ${IAtooldir}/sam1 |tr " " "," |cut -d"," -f 1,3-7 > ${IAtooldir}/sam2.csv
sort ${IAtooldir}/sam2.csv | uniq > ${IAtooldir}/IA_tool_output_file.csv

rm -f  ${IAtooldir}/sam1 ${IAtooldir}/sam2.csv


##############################################################################################################################
# End of script
##############################################################################################################################


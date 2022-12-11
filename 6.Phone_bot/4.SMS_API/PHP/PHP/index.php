<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<html>
 <head>
  <meta http-equiv="Content-Type" content="text/html; charset=utf-8">
  <title>Супер голосование</title>
 </head>
 <body>

    <?php 
      #settings for vore
        require("count.php");
        $number = "79*****";
        $date_gt="2022-12-03T22:00:27.00Z";
        $date_lt="2022-12-04T02:00:27.00Z";
        $first="cat";
        $second="dog";
    ?>

    <table  width="100%" border="1" cellspacing="1" cellpadding="5">
        <tr>
          <th colspan="3" align="center" >Голосование!!!</th>
        </tr>
        <tr  align="center" >
            <td width="33%">
                Котики <br />
                <img src="cat.png" alt="Cat" />
            </td>
            <td width="33%"  rowspan="2"> Всего СМС 

            <?php
               $result = count_votes($number, $date_gt, $date_lt , $first,$second);
                echo array_sum($result);

           


              # echo json_encode($result, JSON_UNESCAPED_UNICODE | JSON_UNESCAPED_SLASHES | JSON_PRETTY_PRINT);
             ?>


            </td>
            <td width="33%">
                Песики <br />
                <img src="dog.png" alt="Dog"  />
            </td>
        </tr>
        <tr align="center" >
            <td  width="33%"> <?php echo $result[0] ?></td>

            <td width="33%"><?php echo $result[1] ?></td>
        </tr>

      </table>
 </body>
</html>
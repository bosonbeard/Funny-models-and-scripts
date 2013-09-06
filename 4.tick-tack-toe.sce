// Tick-tack-toe
//Tic Tac Toe, on the basis of scilab
//Start here

global ffield;  
global Win;
ffield=0;

//the creation of the game window
mw=figure();   
set(mw,'position',[20,40,500,450]);
set(mw,'figure_name','Tick-tack-toe');

//status Bar
label1=uicontrol('Style', 'text', 'Position', [80,400,250,20], 'String',..
'Game in progress (click on the square button)','HorizontalAlignment','left');
//the creation of the game window
ubutton_c=uicontrol(mw,'Style','pushbutton','position',[80,380,120,20]..
,'String','Restart game','CallBack','newgame()');

//the creation of the playing field
ubutton=list([1:9]);
for i=1:9 
    num=string(i)
    y=ceil(i/3);
    x=i-((y-1)*3);
    ubutton(i)=uicontrol(mw,'Style','pushbutton','position',[80*x+2,80*y,80,80]..
    ,'String',' ','CallBack','press_button('+num+')');
end;



function y=press_button(button_num)
    //gaming activities
    global ffield;
    global Win;
    // Human
    Button_Value=get(ubutton(button_num),'String');
    if Button_Value==" " then
        set(ubutton(button_num),'String',"X");
        ffield=ffield+1;
    end
    // AI 
    if ffield<9 then 
        ct=comp_turn();  //get random action of AI
        buf=get(ubutton(ct),'String');
        while buf ~= " " do
            ct=comp_turn();  //get random action of AI
            buf=get(ubutton(ct),'String');
        end
        set(ubutton(ct),'String',"0"); // Ai chooses cell
        ffield=ffield+1;  //counter filled cells
        Winner() //find game winner
endfunction

function R=comp_turn()
    // Computer opponent action (random action)
    R = grand(1,1,"poi",4);
    if R>9 then R=9;    end;
    if R<1 then R=1;    end;
endfunction

function Winner()
    //find game winner function
    
    res=0;
    pw=0;
    cw=0;
    plfield=hypermat([3,3]);   // results matrix for human
    cmfield=hypermat([3,3]);   // results matrix for AI
    
    // check game field
    for ck=1:9 Button_Value=get(ubutton(ck),'String');
        j=ceil(ck/3);
        i=ck-((j-1)*3);
        if  Button_Value=="X"  then plfield(j,i)=1;  end 
        if  Button_Value=="0" then cmfield(j,i)=1; end 
    end   


    // check human results
    pb=pob_diag(plfield,3) ;//processing second diag.
    sm=prod(plfield,1)+prod(plfield,2)';//processing  matrix
    if sum(sm)==1 then res=1;   end; //check winning the hor. and vert. field
    if diag(plfield,0)==1 then res=1;   end; //check winning field main diag.
    if  pb==1 then res=1;   end; //check winning field second diag.
    
    // check AI results
    pb=pob_diag(cmfield,3) //processing second diag.
    sm=prod(cmfield,1)+prod(cmfield,2)';//processing matrix
    if sum(sm)==1 then res=2;   end; // check winning the hor. and vert. field
    if diag(cmfield,0)==1 then res=2;   end; //check winning field main diag.
    if  pb==1 then res=2;   end; //check winning field second diag.

 
    //  Deciding the winner
    if  res==1 then set(label1,'String',"You Win");   end 
    if  res==2 then set(label1,'String',"Computer Win");   end 
    if  (ffield>=9) & (res==0) then set(label1,'String',"No Winner");   end 
endfunction

function mult=pob_diag(A,N)
      //  analysis of the secondary diagonal matrix
    mult=1;
    for i=1:N

        mult=mult*A(i,N+1-i);
    end
endfunction

function mult=find_one(Win)
   // analysis of the columns of the matrix
    mult=1;
    prod(Win,1)+prod(Win,2)'
    for i=1:2
        mult=mult+i;
    end
endfunction

function newgame()
    // restart game (reset values)
    
    global ffield;
    global Win;
    global res;
    for i=1:9 
        y=ceil(i/3);
        x=i-((y-1)*3);
        set(ubutton(i),'String',' ');
    end;
    ffield=0;
    res=0
    Win=0;
    set(label1, 'String','Game in progress (click on the square button)');
endfunction

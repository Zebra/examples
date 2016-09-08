package com.symbol.removeet1link;

import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.util.Log;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;

public class MainActivity extends AppCompatActivity {
    public static String TAG = "removeET1Link";
    String configfile = "/enterprise/usr/enterprisehomescreen.xml";

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);


            String aBuffer = "";
            File myFile = new File(configfile);
            FileInputStream fIn = null;
            BufferedReader myReader = null;
            try {
                fIn = new FileInputStream(
                        myFile);
            } catch (FileNotFoundException e) {
                Log.d(TAG,"Failed to find config file");
            }
            try {
                myReader = new BufferedReader(
                        new InputStreamReader(
                                fIn));
                String aDataRow = "";

                //Read in current config file, add each line to a buffer excluding et1 video link.
                try {
                    while ((aDataRow = myReader
                            .readLine()) != null) {

                        if (!aDataRow
                                .contains("label=\"ET1 Video\"")) {
                            aBuffer += aDataRow
                                    + "\n";

                        }
                    }
                    myReader.close();
                } catch (IOException e) {
                    Log.d(TAG,e.getMessage());
                }

            } catch (Exception e) {
                Log.d(TAG,"Failed read config file");
            }

            File outFile = new File(configfile);
            try {
                outFile.createNewFile();
                FileOutputStream fOut = new FileOutputStream(
                        outFile);
                OutputStreamWriter myOutWriter = new OutputStreamWriter(
                        fOut);
                myOutWriter.append(aBuffer);
                myOutWriter.close();
                fOut.close();
            } catch (IOException e) {
                Log.d(TAG,e.getMessage());
            }
        this.finish();
    }
}

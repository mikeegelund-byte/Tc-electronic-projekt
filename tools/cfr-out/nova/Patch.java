/*
 * Decompiled with CFR 0.152.
 */
package nova;

import java.awt.BorderLayout;
import java.awt.Component;
import java.awt.GridBagConstraints;
import java.awt.GridBagLayout;
import java.awt.Point;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.MouseEvent;
import java.awt.event.MouseListener;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.FileWriter;
import java.io.IOException;
import java.io.InputStream;
import java.io.PrintWriter;
import java.util.List;
import java.util.Vector;
import javax.swing.Box;
import javax.swing.JButton;
import javax.swing.JComboBox;
import javax.swing.JFileChooser;
import javax.swing.JPanel;
import javax.swing.JRootPane;
import javax.swing.SwingUtilities;
import nova.Block;
import nova.Boost;
import nova.Comp;
import nova.Constants;
import nova.Delay;
import nova.Drive;
import nova.EQ;
import nova.Gate;
import nova.Global;
import nova.MidiInterface;
import nova.Modulation;
import nova.Nibble;
import nova.NibbleException;
import nova.NovaManager;
import nova.Pitch;
import nova.PrintablePanel;
import nova.Reverb;

/*
 * This class specifies class file version 49.0 but uses Java 6 signatures.  Assumed Java 6.
 */
public class Patch
extends PrintablePanel
implements ActionListener,
MouseListener {
    protected static final int PATCH_SIZE = 520;
    protected static final int MESSAGE_ID_OFFSET = 6;
    protected static final int PRESET_NUMBER_OFFSET = 8;
    protected static final int PRESET_NAME_OFFSET = 10;
    protected static final int PRESET_NAME_LENGTH = 24;
    protected static final int CHECKSUM_OFFSET = 518;
    protected static final int CHECKSUM_START = 34;
    protected static final int CHECKSUM_END = 517;
    protected String[] exp_pedal_valueList;
    protected byte[] data;
    private String preset_name;
    private int message_id;
    private int checksum;
    private int bankN;
    private int presN;
    private int numcode;
    private File saveFile;
    private boolean isVariation = false;
    protected static final int NIBBLE_LENGTH = 121;
    protected Vector<Nibble> nibbles;
    protected Vector<Block> blocks;
    JRootPane pane;
    JPanel titlePane;
    Global global;
    JButton closeButton = new JButton("Close");
    JButton saveButton = new JButton("Save SysEx");
    JButton saveXML = new JButton("Export XML");
    JButton saveHTML = new JButton("Export HTML");
    JComboBox send = new JComboBox();
    NovaManager parent;

    public Patch(NovaManager novaManager, byte[] byArray, boolean bl) {
        int n;
        this.parent = novaManager;
        this.setLog(novaManager.log);
        this.isVariation = true;
        this.data = new byte[520];
        for (n = 0; n < 520; ++n) {
            this.data[n] = byArray[n];
        }
        this.nibbles = new Vector();
        try {
            for (n = 0; n < 121; ++n) {
                byte[] byArray2 = new byte[4];
                for (int i = 0; i < 4; ++i) {
                    byArray2[i] = this.data[34 + 4 * n + i];
                }
                this.nibbles.add(new Nibble(this, byArray2, 34 + 4 * n));
            }
        }
        catch (NibbleException nibbleException) {
            this.parent.log(nibbleException.getMessage());
        }
        this.titlePane = new JPanel();
        this.titlePane.setLayout(new BorderLayout());
        this.titlePane.setOpaque(false);
    }

    public Patch(NovaManager novaManager, byte[] byArray) {
        Object object;
        int n;
        this.parent = novaManager;
        this.setLog(novaManager.log);
        this.data = new byte[520];
        for (n = 0; n < 520; ++n) {
            this.data[n] = byArray[n];
        }
        this.nibbles = new Vector();
        try {
            for (n = 0; n < 121; ++n) {
                object = new byte[4];
                for (int i = 0; i < 4; ++i) {
                    object[i] = this.data[34 + 4 * n + i];
                }
                this.nibbles.add(new Nibble(this, (byte[])object, 34 + 4 * n));
            }
        }
        catch (NibbleException nibbleException) {
            this.parent.log(nibbleException.getMessage());
        }
        this.titlePane = new JPanel();
        this.titlePane.setLayout(new BorderLayout());
        this.titlePane.setOpaque(false);
        this.refresh();
        this.setName(this.preset_name);
        this.blocks = new Vector();
        this.global = new Global(this);
        object = new Drive(this);
        Comp comp = new Comp(this);
        Gate gate = new Gate(this);
        Boost boost = new Boost(this);
        EQ eQ = new EQ(this);
        Modulation modulation = new Modulation(this);
        Pitch pitch = new Pitch(this);
        Delay delay = new Delay(this);
        Reverb reverb = new Reverb(this);
        this.blocks.add(this.global);
        this.blocks.add((Block)object);
        this.blocks.add(comp);
        this.blocks.add(gate);
        this.blocks.add(boost);
        this.blocks.add(eQ);
        this.blocks.add(modulation);
        this.blocks.add(pitch);
        this.blocks.add(delay);
        this.blocks.add(reverb);
        this.closeButton.addActionListener(this);
        this.closeButton.setActionCommand("close");
        this.saveButton.addActionListener(this);
        this.saveButton.setActionCommand("save");
        this.saveXML.addActionListener(this);
        this.saveXML.setActionCommand("saveXML");
        this.saveHTML.addActionListener(this);
        this.saveHTML.setActionCommand("saveHTML");
        this.refreshNovasList();
        this.send.setActionCommand("send");
        this.send.setEnabled(false);
        Box box = Box.createHorizontalBox();
        box.add(eQ);
        box.add(modulation);
        box.add(pitch);
        box.add(delay);
        box.add(reverb);
        Box box2 = Box.createHorizontalBox();
        box2.add(this.global);
        box2.add((Component)object);
        box2.add(comp);
        box2.add(gate);
        box2.add(boost);
        Box box3 = Box.createVerticalBox();
        box3.add(box2);
        box3.add(Box.createVerticalStrut(10));
        box3.add(box);
        JPanel jPanel = new JPanel();
        jPanel.setOpaque(false);
        jPanel.add(this.closeButton);
        jPanel.add(this.saveButton);
        jPanel.add(this.saveXML);
        jPanel.add(this.saveHTML);
        jPanel.add(this.send);
        this.setLayout(new GridBagLayout());
        GridBagConstraints gridBagConstraints = new GridBagConstraints();
        gridBagConstraints.gridx = 0;
        gridBagConstraints.gridy = 0;
        gridBagConstraints.weightx = 1.0;
        gridBagConstraints.weighty = 0.0;
        gridBagConstraints.fill = 2;
        gridBagConstraints.anchor = 19;
        this.add((Component)this.titlePane, gridBagConstraints);
        gridBagConstraints.weighty = 0.0;
        ++gridBagConstraints.gridy;
        gridBagConstraints.fill = 0;
        this.add((Component)box3, gridBagConstraints);
        ++gridBagConstraints.gridy;
        gridBagConstraints.weighty = 1.0;
        gridBagConstraints.fill = 2;
        gridBagConstraints.anchor = 20;
        this.add((Component)jPanel, gridBagConstraints);
        this.setOpaque(true);
        this.setBackground(Constants.grisvert);
    }

    boolean isVariation() {
        return this.isVariation;
    }

    void refresh() {
        this.checksum = this.data[518];
        this.message_id = this.data[6];
        this.numcode = this.data[8];
        this.bankN = (this.numcode - 31) / 3;
        this.presN = (this.numcode - 31) % 3 + 1;
        this.preset_name = "";
        for (int i = 0; i < 24 && this.data[10 + i] != 0; ++i) {
            this.preset_name = this.preset_name + (char)this.data[10 + i];
        }
    }

    void setExpressionPedalValueList() {
        int n;
        int n2 = this.getNibble(41).getValue();
        int n3 = this.getNibble(57).getValue();
        int n4 = this.getNibble(105).getValue();
        this.exp_pedal_valueList = new String[20];
        this.exp_pedal_valueList[0] = "Off";
        for (n = 0; n < 2; ++n) {
            this.exp_pedal_valueList[1 + n] = Constants.DRV_EXP[n];
        }
        for (n = 0; n < 4; ++n) {
            this.exp_pedal_valueList[3 + n] = Constants.MOD_EXP[n2 * 4 + n];
        }
        for (n = 0; n < 6; ++n) {
            this.exp_pedal_valueList[7 + n] = Constants.DLY_EXP[n3 * 6 + n];
        }
        for (n = 0; n < 4; ++n) {
            this.exp_pedal_valueList[13 + n] = Constants.REV_EXP[n];
        }
        for (n = 0; n < 3; ++n) {
            this.exp_pedal_valueList[17 + n] = Constants.PIT_EXP[n4 * 3 + n];
        }
    }

    Vector<String> getExpressionPedalValueList() {
        this.setExpressionPedalValueList();
        Vector<String> vector = new Vector<String>();
        for (int i = 0; i < 20; ++i) {
            if (this.exp_pedal_valueList[i] == "") continue;
            vector.add(this.exp_pedal_valueList[i]);
        }
        return vector;
    }

    String getOriginalName() {
        return this.preset_name;
    }

    void setPresetCode(int n) {
        this.numcode = n;
    }

    void setBankNumber(int n) {
        this.bankN = n;
    }

    void setPresetNumber(int n) {
        this.presN = n;
    }

    void setPresetName(String string) {
        this.preset_name = string;
    }

    int getPresetCode() {
        return this.numcode;
    }

    int getOriginalBankNumber() {
        return this.bankN;
    }

    int getOriginalPresetNumber() {
        return this.presN;
    }

    JPanel getTitlePane() {
        return this.titlePane;
    }

    void refreshTitle() {
        this.global.refreshTitle();
        this.parent.refreshTabName(this.preset_name);
    }

    void saveValues() {
        int n;
        this.data[8] = (byte)this.numcode;
        for (n = 0; n < 24; ++n) {
            this.data[10 + n] = n < this.preset_name.length() ? (byte)this.preset_name.charAt(n) : (byte)0;
        }
        for (n = 0; n < this.nibbles.size(); ++n) {
            this.getNibble(n).save(this.data);
        }
        this.data[518] = (byte)this.calculateChecksum();
    }

    int calculateChecksum() {
        return Patch.calculateChecksum(this.data);
    }

    static int calculateChecksum(byte[] byArray) {
        int n = 0;
        for (int i = 34; i <= 517; ++i) {
            n += byArray[i];
        }
        return n % 128;
    }

    public static boolean check(byte[] byArray) {
        boolean bl = true;
        if (bl && byArray.length != 520) {
            bl = false;
        }
        if (bl && byArray[0] != MidiInterface.aPreset[0]) {
            bl = false;
        }
        if (bl && byArray[1] != MidiInterface.aPreset[1]) {
            bl = false;
        }
        if (bl && byArray[2] != MidiInterface.aPreset[2]) {
            bl = false;
        }
        if (bl && byArray[3] != MidiInterface.aPreset[3]) {
            bl = false;
        }
        if (bl && byArray[5] != MidiInterface.aPreset[5]) {
            bl = false;
        }
        if (bl && byArray[6] != MidiInterface.aPreset[6]) {
            bl = false;
        }
        if (bl && byArray[7] != MidiInterface.aPreset[7]) {
            bl = false;
        }
        if (bl && byArray[519] != -9) {
            bl = false;
        }
        if (bl && byArray[518] != Patch.calculateChecksum(byArray)) {
            bl = false;
        }
        return bl;
    }

    @Override
    public void actionPerformed(ActionEvent actionEvent) {
        if (actionEvent.getActionCommand().equals("close")) {
            this.closeTab();
        } else if (actionEvent.getActionCommand().equals("save")) {
            this.saveValues();
            this.saveSysEx();
        } else if (actionEvent.getActionCommand().equals("saveXML")) {
            this.saveValues();
            this.saveXML();
        } else if (actionEvent.getActionCommand().equals("saveHTML")) {
            this.saveValues();
            this.saveHTML();
        } else if (actionEvent.getActionCommand().equals("send") && this.send.getSelectedIndex() > 0) {
            Integer n = (Integer)this.send.getSelectedItem();
            this.data[4] = n.byteValue();
            this.saveValues();
            this.parent.send(this.data);
        }
    }

    void refreshNovasList() {
        this.send.removeActionListener(this);
        this.send.removeAllItems();
        this.send.addItem("Send to Nova");
        List list = this.parent.getNovas();
        if (list != null) {
            for (int i = 0; i < list.size(); ++i) {
                this.send.addItem(list.get(i));
            }
        }
        this.send.addActionListener(this);
    }

    public void writeBytesToFile(File file, byte[] byArray) {
        try {
            FileOutputStream fileOutputStream = new FileOutputStream(file);
            byte[] byArray2 = new byte[]{-16, 0, 32, 31, 127, 99, 69, 1, 31, 0, -9};
            fileOutputStream.write(byArray2);
            fileOutputStream.close();
            this.parent.logSilent("" + byArray2.length + " bytes written to " + file.getName());
        }
        catch (FileNotFoundException fileNotFoundException) {
            this.parent.log("FileNotFoundException : " + fileNotFoundException);
        }
        catch (IOException iOException) {
            this.parent.log("IOException : " + iOException);
        }
    }

    public static byte[] getBytesFromFile(File file) throws IOException {
        int n;
        FileInputStream fileInputStream = new FileInputStream(file);
        long l = file.length();
        if (l < 520L) {
            System.err.println("Input File is too short");
        }
        byte[] byArray = new byte[(int)l];
        int n2 = 0;
        for (n = 0; n < byArray.length && (n2 = ((InputStream)fileInputStream).read(byArray, n, byArray.length - n)) >= 0; n += n2) {
        }
        if (n < byArray.length) {
            throw new IOException("Could not completely read file " + file.getName());
        }
        ((InputStream)fileInputStream).close();
        return byArray;
    }

    Nibble getNibble(int n) {
        return this.nibbles.elementAt(n);
    }

    String toHTML() {
        Block block;
        int n;
        String string = "";
        string = string + "<HTML>\n";
        string = string + "  <BODY>\n";
        string = string + "    <H1>Preset #" + this.numcode + " [" + Constants.PRESET[this.numcode] + "] -- " + this.preset_name + "</H1>\n";
        string = string + "    <TABLE border=\"1\" cellpadding=\"10\" width=\"1000\">\n";
        string = string + "      <TR>\n";
        for (n = 0; n < 5; ++n) {
            block = this.blocks.elementAt(n);
            string = string + block.toHTML();
        }
        string = string + "      </TR>\n";
        string = string + "      <TR>\n";
        for (n = 5; n < this.blocks.size(); ++n) {
            block = this.blocks.elementAt(n);
            string = string + block.toHTML();
        }
        string = string + "      </TR>\n";
        string = string + "    </TABLE>\n";
        string = string + "  </BODY>\n";
        string = string + "</HTML>\n";
        return string;
    }

    public String toXML() {
        String string = "";
        string = string + "<patch vendorId=\"00201F\" vendor=\"TC Electronic\" ";
        string = string + "modelId=\"63\" model=\"Nova System\" ";
        string = string + " n=\"" + this.numcode;
        string = string + "\" bank=\"" + this.bankN;
        string = string + "\" preset=\"" + this.presN;
        string = string + "\" name=\"" + this.preset_name;
        string = string + "\">\n";
        for (int i = 0; i < this.blocks.size(); ++i) {
            Block block = this.blocks.elementAt(i);
            string = string + block.toXML();
        }
        string = string + "</patch>";
        return string;
    }

    void closeTab() {
        this.parent.removeTab(this);
    }

    void saveSysEx() {
        JFileChooser jFileChooser = this.parent.getFileChooser();
        int n = jFileChooser.showSaveDialog(this);
        if (n == 0) {
            File file = jFileChooser.getSelectedFile();
            try {
                FileOutputStream fileOutputStream = new FileOutputStream(file);
                fileOutputStream.write(this.data);
                fileOutputStream.close();
            }
            catch (IOException iOException) {
                iOException.printStackTrace();
            }
        }
    }

    void saveXML() {
        JFileChooser jFileChooser = this.parent.getFileChooser();
        int n = jFileChooser.showSaveDialog(this);
        if (n == 0) {
            File file = jFileChooser.getSelectedFile();
            try {
                FileWriter fileWriter = new FileWriter(file);
                PrintWriter printWriter = new PrintWriter(fileWriter);
                printWriter.println(this.toXML());
                printWriter.close();
            }
            catch (IOException iOException) {
                iOException.printStackTrace();
            }
        }
    }

    void saveHTML() {
        JFileChooser jFileChooser = this.parent.getFileChooser();
        int n = jFileChooser.showSaveDialog(this);
        if (n == 0) {
            File file = jFileChooser.getSelectedFile();
            try {
                FileWriter fileWriter = new FileWriter(file);
                PrintWriter printWriter = new PrintWriter(fileWriter);
                printWriter.println(this.toHTML());
                printWriter.close();
            }
            catch (IOException iOException) {
                iOException.printStackTrace();
            }
        }
    }

    void setStandalone(boolean bl) {
        this.send.setEnabled(!bl);
    }

    @Override
    public void mouseClicked(MouseEvent mouseEvent) {
        Component component = (Component)mouseEvent.getSource();
        Point point = component.getLocationOnScreen();
        int n = mouseEvent.getX() + point.x;
        int n2 = mouseEvent.getY() + point.y;
        if (n < 0) {
            n = 0;
        }
        if (n2 < 0) {
            n2 = 0;
        }
        this.parent.en.update(this, n, n2);
    }

    @Override
    public void mousePressed(MouseEvent mouseEvent) {
    }

    @Override
    public void mouseReleased(MouseEvent mouseEvent) {
    }

    @Override
    public void mouseEntered(MouseEvent mouseEvent) {
    }

    @Override
    public void mouseExited(MouseEvent mouseEvent) {
    }

    public static void main(String[] stringArray) {
        if (stringArray.length != 1) {
            System.out.println("Syntax : java nova.Patch <Sysex inputFile>");
        } else {
            File file = new File(stringArray[0]);
            try {
                final byte[] byArray = Patch.getBytesFromFile(file);
                SwingUtilities.invokeLater(new Runnable(){

                    public void run() {
                        NovaManager novaManager = new NovaManager();
                        novaManager.setEditable(true);
                        novaManager.add(new Patch(novaManager, byArray));
                    }
                });
            }
            catch (IOException iOException) {
                System.err.println("File not found : " + stringArray[1]);
            }
        }
    }
}

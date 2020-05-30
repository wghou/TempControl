/*
Navicat MySQL Data Transfer

Source Server         : ocean
Source Server Version : 50505
Source Host           : 127.0.0.1:3306
Source Database       : autotest

Target Server Type    : MYSQL
Target Server Version : 50505
File Encoding         : 65001

Date: 2020-05-30 14:04:20
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for s_instrumentdata
-- ----------------------------
DROP TABLE IF EXISTS `s_instrumentdata`;
CREATE TABLE `s_instrumentdata` (
  `vTestID` varchar(50) DEFAULT NULL COMMENT '测试编号',
  `vInstrumentID` varchar(50) DEFAULT NULL COMMENT '仪器编号',
  `vItemType` varchar(255) DEFAULT NULL COMMENT '项目类型T/C',
  `vTitularValue` double(11,0) DEFAULT NULL COMMENT '名义值',
  `vTemperature` double DEFAULT NULL COMMENT '电导率对应的温度值',
  `vConductivity` double DEFAULT NULL COMMENT '平均值',
  `vSalinity` double DEFAULT NULL,
  `vTemperatureRaw` int(11) DEFAULT NULL COMMENT ' 原始值',
  `vConductivityRaw` double DEFAULT NULL COMMENT '某个校准点仪器所有数据',
  `vMeasureTime` datetime DEFAULT NULL,
  `vAddTime` datetime DEFAULT NULL,
  `vUpdateTime` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
